using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.infra.efc.Data;
using Microsoft.Extensions.Logging;

namespace eInvoice.infra.efc.Repositories;

internal class PaidInvoicesRepositroy : IPaidInvoiceRepository
{
    private readonly ILogger<PaidInvoicesRepositroy> logger;
    private readonly BillingContext billingContext;

    public PaidInvoicesRepositroy(ILogger<PaidInvoicesRepositroy> logger, BillingContext billingContext)
    {
        this.logger = logger;
        this.billingContext = billingContext;
    }

    public async Task<PaidInvoiceDto?> GetPaidInvoiceAsync(string invoiceId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(invoiceId))
        {
            throw new ArgumentNullException(nameof(invoiceId));
        }

        try
        {
            var invoice = await billingContext.Invoices
                .AsNoTracking()
                .Select(i => new { i.InvoiceNumber, i.BillToParty, i.BillingDate })
                .FirstOrDefaultAsync(i => i.InvoiceNumber != null && i.InvoiceNumber.Equals(invoiceId), cancellationToken) ?? throw new InvalidOperationException($"Invoice with ID {invoiceId} not found.");

            var company = await billingContext.CustomerMasters.AsNoTracking()
                                                              .Where(cm => cm.CustomerNumber != null && cm.CustomerNumber.Equals(invoice!.BillToParty))
                                                              .FirstOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Customer with CustomerNumber {invoice.BillToParty} not found.");

            var companyDetails = await billingContext.CustomerDetails.AsNoTracking()
                                                                     .Where(cd => cd.CompanyId != null && cd.CompanyId.Equals(company!.TaxNumber2))
                                                                     .FirstOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Company Details with ID {company!.TaxNumber2} not found.");

            var items = await billingContext.InvoiceItems.AsNoTracking()
                                                         .Where(ii => ii.AltInvoiceNumber != null && ii.AltInvoiceNumber.StartsWith(invoiceId))
                                                         .ToListAsync(cancellationToken);

            var dto = new PaidInvoiceDto
            {
                InvoiceNumber = invoiceId,
                IssueDate = DateOnly.FromDateTime(invoice!.BillingDate ?? DateTime.Now),
                CompanyId = companyDetails.CompanyId,
                PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                PaymentAmount = Math.Round(items.Sum(i => i.PayPrice) ?? 0, 0),
                IsRegistered = companyDetails.IsRegistered ?? false
            };

            return dto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving paid invoice with ID {InvoiceId}", invoiceId);
            return null;
        }
    }

    public async Task<IEnumerable<string>?> GetUnprocessedPaidInvoicesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var invoices = await billingContext.InvoiceDetails
                .AsNoTracking()
                .Where(i => i.InvoiceNumber != null
                            && i.IsPaid == true
                            && i.PaidFiscalizationDateTime == null)
                .Select(id => id.InvoiceNumber!)
                .ToListAsync();

            return invoices;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving unprocessed paid invoices");
            throw;
        }
    }

    public async Task<bool> UpdatePaidInvoiceAsync(PaidInvoiceDto? paidInvoiceDto, CancellationToken cancellationToken = default)
    {
        if (paidInvoiceDto is null 
            || string.IsNullOrWhiteSpace(paidInvoiceDto.InvoiceNumber))
            throw new ArgumentNullException(nameof(paidInvoiceDto));
        var invoiceDetails = billingContext.InvoiceDetails
            .FirstOrDefault(i => i.InvoiceNumber != null && i.InvoiceNumber.Equals(paidInvoiceDto.InvoiceNumber)) ?? throw new InvalidOperationException("Invoice not found");

        try
        {
            invoiceDetails.PaidConfirmationMessage = paidInvoiceDto.Message;
            invoiceDetails.PaidFiscalizationDateTime = paidInvoiceDto.FiscalizationDateTime;

            return await billingContext.SaveChangesAsync(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating paid invoice with ID {InvoiceId}", paidInvoiceDto.InvoiceNumber);
            return false;
        }
    }
}
