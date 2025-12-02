global using eInvoice.domain.Adapters;
using eInvoice.domain.Models;

public static class Globals
{
    public const string SCHEME_ID = "9934";
    public const string CUSTOMIZATION_ID = "urn:cen.eu:en16931:2017#compliant#urn:mfin.gov.hr:cius-2025:1.0#conformant#urn:mfin.gov.hr:ext-2025:1.0";
    public const string INVOICE_TYPE_CODE = "380";
    public const string COUNTRY = "HR";
    public const string TAX_SCHEME_ID = "VAT";
    public const string PAYMENT_MEANS_CODE = "42";
    public const string PAYMENT_CHANNEL_CODE = "IBAN";
    public const string INSTRUCTION_NOTE = "Plaćanje po računu";
    public const string PAYMENT_ID = "HR99";
    // lagacy Billing keeps this hardcoded, according to https://unece.org/trade/documents/revision-17-annexes-i-iii it stands for HOUR
    public const string UNIT_CODE = "HUR"; 
    public const string UNIT_CODE_LIST_ID = "UN/ECE rec 20 8e";
    public const string UNIT_CODE_LIST_AGENCY_ID = "6";
    public const string LIST_ID = "CG";
}