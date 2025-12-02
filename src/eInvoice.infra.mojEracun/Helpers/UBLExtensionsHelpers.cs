using eInvoice.domain.Models;

internal static class UBLExtensionsHelpers
{

    /// <summary>
    /// [HR-BR-26] Račun koji sadržava stavku računa (BG-25), popust na razini dokumenta (BG-20) ili trošak na razini dokumenta (BG-21), 
    /// gdje je kod kategorije PDV-a (BT-151, BT-95 ili BT-102) „oslobođeno od PDV-a“ mora u za svaki razlog oslobođenja od PDV-a ili kod razloga oslobođenja od PDV-a sadržavati HR raspodjelu oslobođenja od PDV (HR-BG-2)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool IsHrBr26(this TaxCategoryDto item)
    {
        if (item.Id.Equals("E"))
            return true;

        return false;
    }
}