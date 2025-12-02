using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class VslVoyLine
{
    public string VesselId { get; set; } = null!;

    public long LineOpGkey { get; set; }

    public string LineOpId { get; set; } = null!;

    public string LineInVoyNbr { get; set; } = null!;

    public string? LineOutVoyNbr { get; set; }
}
