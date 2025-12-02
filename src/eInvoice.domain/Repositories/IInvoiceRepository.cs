using eInvoice.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.domain.Repositories
{
    public interface IInvoiceRepository
    {
        Task<InvoiceDto> GetInvoiceAsync(string invoiceId);
        Task<Attachment?> GetInvoiceAttachmentsAsync(string invoiceId);
        Task<IList<ReadyInvoiceDto>> GetReadyInvoicesAsync();
        Task UpdateInvoiceStatus(UpdateElectronicInvoice updateElectronicInvoice, CancellationToken cancellationToken = default);
    }
}
