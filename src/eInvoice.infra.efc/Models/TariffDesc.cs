using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

[Table("TariffDesc")]
public partial class TariffDesc
{
    public int Id { get; set; }

    public string Tariff { get; set; } = null!;

    public string Event { get; set; } = null!;

    public string? Description { get; set; }

    public string Language { get; set; } = null!;
    public Tariff? TariffNavigation { get; set; }
}
