namespace eInvoice.infra.mojEracun.Dtos
{
    public class MarkPaidResponse
    {
        public string? TraceId { get; set; }
        public string? Message { get; set; }
        public DateTimeOffset? FiscalizationTimestamp { get; set; }
        public string? EncodedXml { get; set; }
        public TaxAdministrationSpecific? TaxAdministrationSpecifics { get; set; }
    }

    public class TaxAdministrationSpecific
    {
        public string? Code { get; set; }
        public string?  Description { get; set; }
    }
}
