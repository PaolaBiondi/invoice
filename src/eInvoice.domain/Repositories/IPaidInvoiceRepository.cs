using eInvoice.domain.Models;

namespace eInvoice.domain.Repositories
{
    public interface IPaidInvoiceRepository
    {
        Task<PaidInvoiceDto?> GetPaidInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);
        Task<bool> UpdatePaidInvoiceAsync(PaidInvoiceDto? paidInvoiceDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>?> GetUnprocessedPaidInvoicesAsync(CancellationToken cancellationToken = default);
    }
}
