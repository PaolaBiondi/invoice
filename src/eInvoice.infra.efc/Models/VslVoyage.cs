using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class VslVoyage
{
    public long Gkey { get; set; }

    public string Id { get; set; } = null!;

    public string VesselId { get; set; } = null!;

    public string? CustomsId { get; set; }

    public string CarrierMode { get; set; } = null!;

    public long? VisitNbr { get; set; }

    public string Phase { get; set; } = null!;

    public long? OperatorGkey { get; set; }

    public long CpxGkey { get; set; }

    public long? FcyGkey { get; set; }

    public long? NextFcyGkey { get; set; }

    public DateTime? Ata { get; set; }

    public DateTime? Atd { get; set; }

    public long? CvcvdGkey { get; set; }

    public string Name { get; set; } = null!;

    public string? IbVyg { get; set; }

    public string? ObVyg { get; set; }

    public string? Status { get; set; }

    public string Service { get; set; } = null!;

    public DateTime? TimeOffManifest { get; set; }

    public DateTime? StartWork { get; set; }

    public DateTime? EndWork { get; set; }

    public long CarrierGkey { get; set; }

    public string? RadioCallSign { get; set; }
}
