namespace eInvoice.domain.Models
{
    public class InvoiceLineDto
    {
        public int ItemNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal LineExtensionAmount { get; set; }
        public string TariffDescription { get; set; } = null!;
        public string ServiceMaterial { get; set; } = null!;
        public decimal? PayPrice { get; set; }
        public decimal? PayBasicPrice { get; set; }
        public TaxCategoryDto TaxCategory { get; set; } = null!;
        public CommodityClassificationDto CommodityClassification { get; set; } = null!;
    }
}
