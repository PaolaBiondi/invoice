using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace eInvoice.domain.Models
{
    public class InvoiceDto
    {
        public string Id { get; set; } = null!;
        public string? ProfileId { get; set; }
        public DateTime IssueDateTime { get; set; }
        public DateTime BaselineDateTime { get; set; }
        public string Currency { get; set; } = null!;
        public bool CopyIndicator { get; set; } = false;
        public BillingReferenceDto? BillingReference { get; set; }
        public CompanyDto Supplier { get; set; } = null!;
        public CompanyDto Customer { get; set; } = null!;
        public BankDto BankData { get; set; } = null!;
        public MonetaryTotalDto MonetaryTotal { get; set; } = null!;
        public TaxTotalDto TaxTotalDto { get; set; } = null!;
        public List<InvoiceLineDto> InvoiceLines { get; set; } = new List<InvoiceLineDto>();
    }
}
