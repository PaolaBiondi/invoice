namespace eInvoice.domain.Models
{
    public class PaidInvoiceDto
    {
        public bool IsRegistered { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateOnly PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateOnly IssueDate { get; set; }
        public string CompanyId { get; set; } = null!;
        public string? Message { get; set; } = null!;
        public DateTimeOffset? FiscalizationDateTime { get; set; }
    }
}
