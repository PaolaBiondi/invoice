using System.ComponentModel.DataAnnotations;

namespace eInvoice.infra.efc.Infra.Models;

public class CustomerDetails
{
    public int Id { get; set; }
    /// <summary>
    /// Company identifier according to schema 9934, i.e. OIB in Croatia. 
    /// </summary>
    [MaxLength(35)]
    public string CompanyId { get; set; } = null!;
    public bool? IsRegistered { get; set; }
    //public string? CountryCode { get; set; } // It seems that country is already present in column CustomerMasterT.CityCountryKey
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? Updated { get; set; }

    // CustomerMasterT doesn't have any key (primary or unique), so we cannot create a proper foreign key relationship
}
