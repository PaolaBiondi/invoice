using eInvoice.domain.Models;

namespace eInvoice.domain.Repositories
{
    public interface IRejectedInvoiceRepository
    {
        Task<RejectedInvoiceDto?> GetRejectedInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);
        Task<bool> UpdateRejectedInvoiceAsync(RejectedInvoiceDto? rejectedInvoiceDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>?> GetUnprocessedRejectedInvoicesAsync(CancellationToken cancellationToken = default);
    }
}
