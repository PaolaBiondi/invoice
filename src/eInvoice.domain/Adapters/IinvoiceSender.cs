using eInvoice.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.domain.Adapters
{
    public interface IinvoiceSender 
    {
        Task<AdapterResponse<UpdateElectronicInvoice>> SendAsync(string message, string destination, CancellationToken cancellationToken = default);
        Task<AdapterResponse<UpdateElectronicInvoice>> SendInvoiceAsync(string invoiceId, bool maskContactinfo = true, CancellationToken cancellationToken = default);
        Task<bool> PingAsync();
        Task<bool> CheckCompanyIdInAms(string companyId, CancellationToken cancellationToken = default); 
        Task<AdapterResponse<PaidInvoiceDto>> MarkPaid(PaidInvoiceDto payload, CancellationToken cancellationToken = default);
        Task<RejectedInvoiceDto> MarkRejected(RejectedInvoiceDto payload, CancellationToken cancellationToken = default);
    }
}
