namespace eInvoice.domain.Models
{
    public class RejectedInvoiceDto
    {
        public bool IsRegistered { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateOnly IssueDate { get; set; }
        public DateOnly RejectionDate { get; set; }
        public string RejectionReasonDescription { get; set; } = null!;
        public string CompanyId { get; set; } = null!;
        public string? Message { get; set; } = null!;
        public DateTimeOffset? FiscalizationDateTime { get; set; }
    }
}
