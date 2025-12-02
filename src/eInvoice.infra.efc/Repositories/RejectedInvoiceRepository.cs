using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.infra.efc.Data;
using eInvoice.infra.efc.Infra.Models;
using Microsoft.Extensions.Logging;

namespace eInvoice.infra.efc.Repositories
{
    internal class RejectedInvoiceRepository : IRejectedInvoiceRepository
    {
        private readonly ILogger<IRejectedInvoiceRepository> logger;
        private readonly BillingContext billingContext;

        public RejectedInvoiceRepository(ILogger<IRejectedInvoiceRepository> logger, BillingContext context)
        {
            this.logger = logger;
            this.billingContext = context;
        }

        public async Task<RejectedInvoiceDto?> GetRejectedInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
            {
                throw new ArgumentNullException(nameof(invoiceId));
            }

            try
            {
                // TODO possibly DRY with GetPaidInvoiceAsync, if there is time, rewrite with mapTo
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

                var dto = new RejectedInvoiceDto
                {
                    InvoiceNumber = invoiceId,
                    IssueDate = DateOnly.FromDateTime(invoice!.BillingDate ?? DateTime.Now),
                    RejectionDate = DateOnly.FromDateTime(DateTime.Now),
                    CompanyId = companyDetails.CompanyId,
                    IsRegistered = companyDetails.IsRegistered ?? false
                };

                return dto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving rejected invoice with ID {InvoiceId}", invoiceId);
                return null;
            }
        }

        public async Task<IEnumerable<string>?> GetUnprocessedRejectedInvoicesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var invoices = await billingContext.InvoiceDetails
                    .AsNoTracking()
                    .Where(i => i.InvoiceNumber != null 
                                && i.IsRejected == true
                                && i.RejectedFiscalizationDateTime == null)
                    .Select(id => id.InvoiceNumber!)
                    .ToListAsync();

                return invoices;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving unprocessed rejected invoices");
                throw;
            }
        }

        public async Task<bool> UpdateRejectedInvoiceAsync(RejectedInvoiceDto? rejectedInvoiceDto, CancellationToken cancellationToken = default)
        {
            if (rejectedInvoiceDto is null
                || string.IsNullOrWhiteSpace(rejectedInvoiceDto.InvoiceNumber))
                throw new ArgumentNullException(nameof(rejectedInvoiceDto));

            var invoiceDetails = billingContext.InvoiceDetails
                .FirstOrDefault(i => i.InvoiceNumber != null && i.InvoiceNumber.Equals(rejectedInvoiceDto.InvoiceNumber)) ?? throw new InvalidOperationException("Invoice not found");

            try
            {
                invoiceDetails.RejectedConfirmationMessage = rejectedInvoiceDto.Message;
                invoiceDetails.RejectedFiscalizationDateTime = rejectedInvoiceDto.FiscalizationDateTime;

                return await billingContext.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating paid invoice with ID {InvoiceId}", rejectedInvoiceDto.InvoiceNumber);
                return false;
            }
        }
    }
}
