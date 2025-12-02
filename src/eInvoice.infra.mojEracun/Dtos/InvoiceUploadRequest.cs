using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.infra.mojEracun.Dtos
{
    public class InvoiceUploadRequest: BaseRequestDto
    {
        public string CompanyBu { get; set; } = String.Empty;
        public string File { get; set; } = null!;
    }
}
