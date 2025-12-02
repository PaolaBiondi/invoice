namespace eInvoice.domain.Models
{
    public class TaxCategoryDto
    {
        public string Id { get; set; } = null!;
        public decimal Percent { get; set; }
        public string Name { get; set; } = null!;
        public string ExemptionReason { get; set; } = null!;
        public string TaxTypeCode { get; set; } = null!; // TODO in legacy code"Zero rated"; is it necessary to include taxtypecode?
    }
}
