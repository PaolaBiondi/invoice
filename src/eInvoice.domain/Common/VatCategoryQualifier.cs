namespace eInvoice.domain.Common
{
    public enum VatCategoryQualifier: ushort
    {
        PDV25 = 1,
        PDV13,
        PDV5,
        Z,
        K,
        G,
        AE,
        E
    }

    public static class VatCategoryQualifierExtensions
    {
        public static string GetCode(this VatCategoryQualifier qualifier)
        {
            return qualifier switch
            {
                VatCategoryQualifier.PDV25 => "Standard rate",
                VatCategoryQualifier.PDV13 => "Standard rate",
                VatCategoryQualifier.PDV5 => "Standard rate",
                VatCategoryQualifier.Z => "Zero rated",
                VatCategoryQualifier.K => "Intra community supply",
                VatCategoryQualifier.G => "Exports",
                VatCategoryQualifier.AE => "VAT Reverse charge",
                VatCategoryQualifier.E => "Exempt from tax",
                _ => throw new ArgumentOutOfRangeException(nameof(qualifier), qualifier, null)
            };
        }
    }
}
