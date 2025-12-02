namespace eInvoice.domain.Models
{
    public class TaxSubtotalDto
    {
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public TaxCategoryDto TaxCategory { get; set; } =null!;
    }
}
