using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

[Table(nameof(EdiinvoicingTo))]
public partial class EdiinvoicingTo
{
    public int Id { get; set; }

    public string? BillToParty { get; set; }

    public bool Active { get; set; }

    public string? Service { get; set; }

    public string? User { get; set; }

    public string? Password { get; set; }

    public DateTime? LastSending { get; set; }

    public DateTime? LastChecking { get; set; }

    public string? StreetName { get; set; }

    public string? BuildingNumber { get; set; }

    public string? CityName { get; set; }

    public string? PostalZone { get; set; }

    public string? Country { get; set; }

    public string? CompanyId { get; set; }

    public string? Email { get; set; }

    public string? BusinessUnit { get; set; }

    public string? SoldToParty { get; set; }

    public string? StpName { get; set; }

    public string? StpAddress { get; set; }

    public string? AltMessage { get; set; }

    public string? AltAddress { get; set; }

    public string? AltUser { get; set; }

    public string? AltPassword { get; set; }
    
    [NotMapped]
    public string? AddressLine => $"{this.StreetName} {BuildingNumber}, {PostalZone} {CityName}";
}
