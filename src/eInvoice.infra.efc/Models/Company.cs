using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Models;

[Table("Company")]
public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
    [NotMapped]
    public string? BuildingNumber => Address.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 1 ? Address.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1] : null;
    [NotMapped]
    public string? AddressLine => $"{Address}, {PostCode} {City}";

}
