using eInvoice.domain.Common;
using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.infra.efc.Data;
using eInvoice.infra.efc.Infra.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace eInvoice.infra.efc.Repositories;

internal class InvoiceRepository : IInvoiceRepository
{
    private readonly ILogger<InvoiceRepository> logger;
    private readonly BillingContext billingContext;

    public InvoiceRepository(ILogger<InvoiceRepository> logger, BillingContext billingContext)
    {
        this.logger = logger;
        this.billingContext = billingContext;
    }

    public async Task<InvoiceDto> GetInvoiceAsync(string invoiceId)
    {
        InvoiceDto invoice = new() { Id = invoiceId };

        var dbInvoice = await billingContext.Invoices.AsNoTracking()
                                                      .Include(i => i.InvoicedUserNavigation)
                                                      .Include(i => i.EdiinvoicingTo)
                                                      .Include(i => i.InvoiceTypeNavigation)
                                                      .ThenInclude(it => it!.VatClauseNavigation)
                                                      .Include(i => i.SapInvoices)
                                                      .ThenInclude(s => s.InvoiceItems)
                                                      .Where(i => i.InvoiceNumber.Equals(invoiceId))
                                                      .FirstOrDefaultAsync() ?? throw new ArgumentNullException($"Invoice {invoiceId} not found.");

        invoice.IssueDateTime = dbInvoice.BillingDate ?? throw new ArgumentException($"{nameof(dbInvoice.BillingDate)} cannot be empty");
        invoice.BaselineDateTime = dbInvoice.BaselineDate ?? throw new ArgumentException($"{nameof(dbInvoice.BaselineDate)} cannot be empty");
        invoice.Currency = dbInvoice.Currency ?? throw new ArgumentException($"{nameof(dbInvoice.Currency)} cannot be empty");
        invoice.CopyIndicator = dbInvoice.Invoiced.Equals(SendingStatusQualifier.JosipNumber) ? true : false;

        await EnrichWithBillingReference(invoice, dbInvoice.CancelledBillingNumber);

        await EnrichWithBusinessProcessAsync(invoice);

        await EnrichWithBankDataAsync(invoice);

        await EnrichWithIssuerCompanyDetailsAsync(invoice, dbInvoice);

        await EnrichWithCustomerDetailsAsync(invoice, dbInvoice.BillToParty);

        await EnrichWithMonetaryDetailsAsync(invoice);

        await EnrichWithTaxExemption(invoice);

        await EnrichWithInvoiceLines(invoice, dbInvoice.Language ?? "HR");

        return invoice;
    }

    private async Task EnrichWithCustomerDetailsAsync(InvoiceDto invoiceDto, string? customerNumber)
    {
        if (string.IsNullOrWhiteSpace(customerNumber))
            throw new ArgumentNullException("Invoice BillToParty N/A");

        var customer = await billingContext.CustomerMasters.Where(c => c.CustomerNumber.Equals(customerNumber))
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Customer details not found.");

        var ediInvoicingto = await billingContext.EdiInvocingTos.Where(c => !string.IsNullOrWhiteSpace(c.BillToParty) && c.BillToParty.Equals(customerNumber))
                                                                .FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Customer not found in {nameof(EdiinvoicingTo)}");

        // CR: financial department, enforce the rule to skip employees from being sent to fisk 2.0
        if (ediInvoicingto.BillToParty!.TrimStart('0').Length <= 3)
            throw new InvalidOperationException("Customer is an employee, skipping eInvoice generation.");

        invoiceDto.Customer = new CompanyDto
        {
            Name = customer.Name1 ?? throw new InvalidOperationException("Customer name not found."),
            Email = ediInvoicingto.Email ?? throw new InvalidOperationException("Customer email not found."),
            TaxNumber = ediInvoicingto.CompanyId ?? throw new InvalidOperationException("Customer tax number not found."),
            Street = ediInvoicingto.StreetName ?? throw new InvalidOperationException("Customer street not found."),
            BuildingNumber = ediInvoicingto.BuildingNumber,
            City = ediInvoicingto.CityName ?? throw new InvalidOperationException("Customer city not found."),
            PostalCode = ediInvoicingto.PostalZone ?? throw new InvalidOperationException("Customer postal code not found."),
            Country = ediInvoicingto?.Country ?? throw new InvalidOperationException("Customer country not found.")
        };
    }

    // TODO merge this with EnrichWithInvoiceLines
    private async Task EnrichWithMonetaryDetailsAsync(InvoiceDto invoiceDto)
    {
        var sapInvoices = await billingContext.Sapinvoices.AsNoTracking()
            .Where(si => !string.IsNullOrWhiteSpace(si.InvoiceNumber) && si.InvoiceNumber.Equals(invoiceDto.Id))
            .Include(si => si.InvoiceItems)
            .ToListAsync();

        var monetaryTotal = new MonetaryTotalDto();
        var taxTotal = new TaxTotalDto();
        sapInvoices.ForEach(si =>
        {
            monetaryTotal.LineExtensionAmount += si.TotalPayAmount;
            monetaryTotal.TaxExclusiveAmount += si.TotalPayAmount;
            monetaryTotal.TaxInclusiveAmount += si.TotalPayAmount + si.TotalPayTax;
            monetaryTotal.PayableAmount += si.TotalPayAmount + si.TotalPayTax;
            taxTotal.TaxAmount += si.TotalPayTax;
        });

        // TODO remove taxCategory is calculated wrong
        var taxSubtotals = sapInvoices.SelectMany(si => si.InvoiceItems)
                                      .Where(ii => ii.Tax.HasValue)
                                      .GroupBy(ii => ii.Tax!.Value)
                                      .Select(g => new TaxSubtotalDto
                                      {
                                          TaxableAmount = g.Sum(ii => ii.PayPrice ?? 0),
                                          TaxAmount = g.Sum(ii => ii.PayTax ?? 0),
                                          TaxCategory = new TaxCategoryDto
                                          {
                                              Id = g.Key == 0m ? "E" : "S",
                                              Name = g.Key == 0m ? "HR:E" : $"HR:PDV{(int)(g.First().Tax ?? 0)}",
                                              Percent = g.Key,
                                              TaxTypeCode = g.Key == 0m ? "Exempt from tax" : "Standard rate"
                                          }
                                      }).ToList();

        var query = from InvoiceAdded in billingContext.InvoiceAdded
                    join Vatclause in billingContext.Vatclauses
                      on (InvoiceAdded.VatClauseId ?? 1) equals Vatclause.Id // Legacy billing, if vatclauseId is null then VatClause.Id = 1 is used, i.e. Oslobođeno temeljem Čl.47. st.1. Zakona o PDV-u i Čl.116. st.3. Pravilnika o PDV-u
                    where InvoiceAdded.InvoiceNumber != null && InvoiceAdded.InvoiceNumber.Equals(invoiceDto.Id)
                    select Vatclause.InvoiceTextLoc;

        var vatClause = await query.FirstOrDefaultAsync();

        foreach (var subtotal in taxSubtotals)
        {
            var taxCategory = subtotal.TaxCategory.Percent switch
            {
                5m => GenerateTaxCategoryDto(VatCategoryQualifier.PDV5),
                13m => GenerateTaxCategoryDto(VatCategoryQualifier.PDV13),
                25m => GenerateTaxCategoryDto(VatCategoryQualifier.PDV25),
                _ when vatClause.Contains("Reverse charge", StringComparison.CurrentCultureIgnoreCase) => GenerateTaxCategoryDto(VatCategoryQualifier.K),
                _ when vatClause.Contains("čl.75 st.3", StringComparison.CurrentCultureIgnoreCase) => GenerateTaxCategoryDto(VatCategoryQualifier.AE),
                _ => GenerateTaxCategoryDto(VatCategoryQualifier.E),
            };

            subtotal.TaxCategory.Id = taxCategory.Id;
            subtotal.TaxCategory.Name = taxCategory.Name;   
            subtotal.TaxCategory.TaxTypeCode = taxCategory.TaxTypeCode;
            subtotal.TaxCategory.ExemptionReason = vatClause;
        }

        taxTotal.TaxSubtotals = taxSubtotals;

        invoiceDto.MonetaryTotal = monetaryTotal;
        invoiceDto.TaxTotalDto = taxTotal;
    }

    private async Task EnrichWithTaxExemption(InvoiceDto invoiceDto)
    {
        var query = from InvoiceAdded in billingContext.InvoiceAdded
                    join Vatclause in billingContext.Vatclauses
                      on InvoiceAdded.VatClauseId equals Vatclause.Id
                    where InvoiceAdded.InvoiceNumber != null && InvoiceAdded.InvoiceNumber.Equals(invoiceDto.Id)
                    select Vatclause.InvoiceTextLoc;
        
        var vatClause = await query.FirstOrDefaultAsync();

        if (vatClause is null)
            return;

        foreach (var taxSubtotal in invoiceDto.TaxTotalDto.TaxSubtotals)
        {
            if (taxSubtotal.TaxCategory.Percent == 0m)
            {
                taxSubtotal.TaxCategory.ExemptionReason = vatClause;
            }
        }
    }

    private async Task EnrichWithIssuerCompanyDetailsAsync(InvoiceDto invoiceDto, Invoice invoice)
    {
        var company = await billingContext.Companies.FirstOrDefaultAsync();

        if (company is null || string.IsNullOrWhiteSpace(company.Name))
            throw new InvalidOperationException("Issuer details not found.");

        var issuerMailAddress = await billingContext.Configurations.Where(c => c.ConfigName == "E_INV_SENDER_MAIL")
                                                                   .Select(c => c.ConfigValue)
                                                                   .FirstOrDefaultAsync();
        invoiceDto.Supplier = new CompanyDto
        {
            Name = company.Name,
            Email = issuerMailAddress,
            TaxNumber = company.TaxNumber ?? throw new InvalidOperationException("Issuer tax number not found."),
            Street = company.Address ?? throw new InvalidOperationException("Issuer street not found."),
            City = company.City ?? throw new InvalidOperationException("Issuer city not found."),
            PostalCode = company.PostCode ?? throw new InvalidOperationException("Issuer postal code not found."),
            BuildingNumber = company.Address.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 1 ? company.Address.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]
                                                                                                          : null,
            Country = "HR"
        };

        if (invoice.InvoicedUserNavigation is null || string.IsNullOrWhiteSpace(invoice.InvoicedUserNavigation.LastName))
            throw new InvalidOperationException("Issuer contact name not found.");
        invoiceDto.Supplier.ContactName = $"{invoice.InvoicedUserNavigation?.FirstName} {invoice.InvoicedUserNavigation?.LastName}";
    }

    private async Task EnrichWithBankDataAsync(InvoiceDto invoiceDto)
    {
        var bankData = await billingContext.BankData
            .AsNoTracking()
            .Select(b => new BankDto
            {
                Iban = b.Ibanid ?? "N/A"
            }).FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(bankData?.Iban))
            throw new InvalidOperationException("Bank data not found.");

        invoiceDto.BankData = bankData;
    }

    private async Task EnrichWithInvoiceLines(InvoiceDto invoiceDto, string language)
    {
        var sapInvoices = await billingContext.Sapinvoices
            .Where(si => !string.IsNullOrWhiteSpace(si.InvoiceNumber) && si.InvoiceNumber.Equals(invoiceDto.Id))
            .Include(si => si.InvoiceItems)
            .ToListAsync();

        int itemNumber = 1;
        foreach (var sap in sapInvoices)
        {
            foreach (var item in sap.InvoiceItems)
            {
                var tarifDesc = await billingContext.TariffDescs.AsNoTracking()
                                                                .FirstOrDefaultAsync(t => t.Tariff.Equals(item.Tariff) && t.Event.Equals(item.Event) && t.Language.StartsWith(language));

                var commodityClassification = await billingContext.CommodityClassifications.AsNoTracking()
                                                                                           .FirstOrDefaultAsync(c => c.Tariff.Equals(item.Tariff) && c.Event.Equals(item.Event)) ?? throw new ArgumentNullException($"CommodityClassification not found for (tariff, event) = ({item.Tariff}, {item.Event})");


                var exemptionReason = item.Tax == 0m ? invoiceDto.TaxTotalDto.TaxSubtotals.FirstOrDefault(ts => ts.TaxCategory.Percent == 0m)?.TaxCategory.ExemptionReason ?? string.Empty : string.Empty;

                var taxCategory = item.Tax switch
                {
                    5m => GenerateTaxCategoryDto(VatCategoryQualifier.PDV5),
                    13m => GenerateTaxCategoryDto(VatCategoryQualifier.PDV13),
                    25m => GenerateTaxCategoryDto(VatCategoryQualifier.PDV25),
                    _ when exemptionReason.Contains("Reverse charge", StringComparison.CurrentCultureIgnoreCase) => GenerateTaxCategoryDto(VatCategoryQualifier.K),
                    _ when exemptionReason.Contains("čl.75 st.3", StringComparison.CurrentCultureIgnoreCase) => GenerateTaxCategoryDto(VatCategoryQualifier.AE),
                    _ => GenerateTaxCategoryDto(VatCategoryQualifier.E),
                };

                taxCategory.Percent = item.Tax ?? 0m;
                taxCategory.ExemptionReason = exemptionReason;

                var invoiceLine = new InvoiceLineDto
                {
                    ItemNumber = itemNumber++,
                    Quantity = Math.Sign(item.Price ?? 0) * (item.BilledQty ?? 0), // legacy code (amount[i] > 0 ? quantity[i] : -quantity[i]).ToString("0.00").Replace(',', '.'),
                    LineExtensionAmount = item.PayPrice ?? 0,
                    TariffDescription = tarifDesc?.Description ?? "N/A",
                    CommodityClassification = new CommodityClassificationDto
                    {
                        ClassificationCode = commodityClassification.ClassificationCode
                    },
                    TaxCategory = taxCategory,
                    ServiceMaterial = item.ServiceMaterial?.TrimStart('0') ?? string.Empty,
                    PayBasicPrice = item.PayBasicPrice ?? 0,
                    PayPrice = item.PayPrice ?? 0
                };

                invoiceDto.InvoiceLines.Add(invoiceLine);

            }
        }
    }

    private TaxCategoryDto GenerateTaxCategoryDto(VatCategoryQualifier vatCategory) => new TaxCategoryDto
    {
        Id = vatCategory.ToString(),
        Name = $"HR:{vatCategory}",
        TaxTypeCode = vatCategory.GetCode(),
    };

    private async Task EnrichWithBusinessProcessAsync(InvoiceDto invoiceDto)
    {
        var pp = await billingContext.InvoiceDetails.Include(id => id.BusinessProcessProfileNavigation)
                                                    .Include(id => id.InvoiceNavigation)
                                                    .Where(id => id.InvoiceNavigation != null && !string.IsNullOrWhiteSpace(id.InvoiceNavigation.InvoiceNumber) && id.InvoiceNavigation.InvoiceNumber.Equals(invoiceDto.Id))
                                                    .Select(id => id.BusinessProcessProfileNavigation.Name)
                                                    .FirstOrDefaultAsync() ?? throw new InvalidOperationException($"ProfileId for invoice {invoiceDto.Id} not found.");
        invoiceDto.ProfileId = pp;
    }

    private async Task EnrichWithBillingReference(InvoiceDto invoiceDto, string? cancelledInvoice)
    {
        if (string.IsNullOrWhiteSpace(cancelledInvoice))
            return;

        var cancelledInvoiceData = await billingContext.Invoices.Where(i => i.InvoiceNumber.Equals(cancelledInvoice))
                                                                .Select(i => new BillingReferenceDto { Id = i.InvoiceNumber!, IssueDate = i.BillingDate ?? DateTime.Now })
                                                                .FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Cancelled invoice {cancelledInvoice} not found.");

        invoiceDto.BillingReference = cancelledInvoiceData;
    }

    public async Task<Attachment?> GetInvoiceAttachmentsAsync(string invoiceId)
    {
        if (string.IsNullOrWhiteSpace(invoiceId))
            throw new ArgumentNullException($"{nameof(invoiceId)} N/A");

        Attachment? invoiceAttachment = null;
        var invoiceAdded = await billingContext.InvoiceAdded
            .AsNoTracking()
            .FirstOrDefaultAsync(att => att.InvoiceNumber != null && att.InvoiceNumber.Equals(invoiceId));

        var billingAttachments = await billingContext.InvoiceAttachment
            .AsNoTracking()
            .Where(att => att.InvoiceNumber != null && att.InvoiceNumber.Equals(invoiceId))
            .ToListAsync();

        if (invoiceAdded is null
            || invoiceAdded.SnapshotPdf is null)
            throw new InvalidOperationException($"Attachment for invoice {invoiceId} not found.");

        invoiceAttachment = new Attachment
        {
            InvoiceId = invoiceId,
            StrBase64 = [(Attachment.GetPdfFileName(invoiceId), Convert.ToBase64String(invoiceAdded.SnapshotPdf), MimeCodeQualifier.PDF)]
        };


        if (invoiceAdded.SnapshotAtt is not null)
        {
            invoiceAttachment.StrBase64.Add((Attachment.GetAttachmentXlsxFileName(invoiceId), Convert.ToBase64String(invoiceAdded.SnapshotAtt), MimeCodeQualifier.PDF));
        }

        if (billingAttachments is not null)
        {
            foreach (var att in billingAttachments)
            {
                invoiceAttachment.StrBase64.Add((att.FileName, Convert.ToBase64String(att.AttachmentXls), MimeCodeQualifier.XLSX));
            }
        }

        return invoiceAttachment;
    }

    /// <summary>
    /// Load invoices which are ready to be sent to eInvoice for fiscalization 2.0
    /// </summary>
    /// <returns></returns>
    public async Task<IList<ReadyInvoiceDto>> GetReadyInvoicesAsync()
    {
        var query = from InvoiceDetail in billingContext.InvoiceDetails
                    join InvoiceAdded in billingContext.InvoiceAdded 
                      on InvoiceDetail.InvoiceNumber equals InvoiceAdded.InvoiceNumber
                    join Invoice in billingContext.Invoices
                      on InvoiceDetail.InvoiceNumber equals Invoice.InvoiceNumber
                    join EdiinvoicingTo in billingContext.EdiInvocingTos
                      on Invoice.BillToParty equals EdiinvoicingTo.BillToParty // retrieve only invoices which have ediinvoicingto defined, e.g. employees should be exempted from sending
                    where InvoiceDetail.InvoiceNumber != null
                          && InvoiceDetail.SendingStatus == SendingStatusQualifier.NotSent
                          && (InvoiceDetail.Updated == null || InvoiceDetail.Updated > DateTimeOffset.UtcNow.AddDays(-3))
                          && (InvoiceAdded.SendingStatus != ((int)SendingStatusQualifier.Sent).ToString()) // in legacy application sendingstatus is string which is causing problem to qualify status 
                    select new ReadyInvoiceDto
                    {
                        InvoiceNumber = InvoiceDetail.InvoiceNumber!
                    };

        return await query.ToListAsync();
    }

    public async Task UpdateInvoiceStatus(UpdateElectronicInvoice updateElectronicInvoice, CancellationToken cancellationToken = default)
    {
        if (updateElectronicInvoice is null
            || string.IsNullOrWhiteSpace(updateElectronicInvoice.InvoiceNumber))
        {
            logger.LogError("Invalid updateElectronicInvoice data.");
            return;
        }

        try
        {
            #region legacy stored procedure call
            var invoice = new SqlParameter("invoiceNumber", updateElectronicInvoice.InvoiceNumber);
            var ogds = new SqlParameter("ogds", updateElectronicInvoice.Ogds);
            var electronicId = new SqlParameter("ElectronicId", updateElectronicInvoice.ElectronicId);
            var created = new SqlParameter("Created", updateElectronicInvoice.Created);
            var status = new SqlParameter("Status", updateElectronicInvoice.StatusId);
            var sent = new SqlParameter("Sent", updateElectronicInvoice.Sent);

            var rowsModified = await billingContext.Database.ExecuteSqlAsync($"dbo.inv_UpdateElectronicInvoice {invoice}, {ogds}, {electronicId}, {created}, {status}, {sent}", cancellationToken);
            #endregion

            // Mark invoice as sent
            var invoiceDetails = await billingContext.InvoiceDetails
                .FirstAsync(id => id.InvoiceNumber != null && id.InvoiceNumber.Equals(updateElectronicInvoice.InvoiceNumber), cancellationToken);

            invoiceDetails.SendingStatus = SendingStatusQualifier.Sent;
            invoiceDetails.Updated = DateTimeOffset.UtcNow;

            await billingContext.SaveChangesAsync(cancellationToken);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating electronic invoice status for {InvoiceNumber}", updateElectronicInvoice.InvoiceNumber);
        }
    }
}
