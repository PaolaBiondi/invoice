namespace eInvoice.domain.Models
{
    public class TaxTotalDto
    {
        public decimal TaxAmount { get; set; }
        public List<TaxSubtotalDto> TaxSubtotals { get; set; } = new List<TaxSubtotalDto>();
    }
}
