using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

public partial class Tariff
{
    public int Id { get; set; }

    [Column("Tariff")]
    [StringLength(10)]
    public string Tariff1 { get; set; } = null!;
    [StringLength(50)]
    public string Event { get; set; } = null!;
    public string? Surcharge { get; set; }
    public string? IsF { get; set; }

    public string? IsE { get; set; }

    public string? Truck { get; set; }

    public string? Rail { get; set; }

    public string? Units { get; set; }

    public string IsImport { get; set; } = null!;

    public string? IsExport { get; set; }

    public string IsTranship { get; set; } = null!;

    public string Hazardous { get; set; } = null!;

    public string Oversize { get; set; } = null!;

    public string Teu { get; set; } = null!;

    public int? GroupDays { get; set; }

    [Column("billing_method")]
    public short? BillingMethod { get; set; }

    public string EventGroup { get; set; } = null!;

    public string? SapdistChannel { get; set; }

    public string? Sapdivision { get; set; }

    public string IsStorage { get; set; } = null!;

    public string? OogEvent { get; set; }

    public string? ImoEvent { get; set; }

    public DateTime? Changed { get; set; }

    public bool? VissibleInWh { get; set; }

    [Column("EES_surcharge")]
    public string? EesSurcharge { get; set; }
}
