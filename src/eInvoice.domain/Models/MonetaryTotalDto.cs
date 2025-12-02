namespace eInvoice.domain.Models
{
    public class MonetaryTotalDto
    {
        // represents total amount, in legacy  billing related to totalAmount variable
        public decimal LineExtensionAmount { get; set; }
        // in legacy billing this is same as LineExtensionAmount
        public decimal TaxExclusiveAmount { get; set; }
        public decimal TaxInclusiveAmount { get; set; }
        public decimal PayableAmount { get; set; }
    }
}
