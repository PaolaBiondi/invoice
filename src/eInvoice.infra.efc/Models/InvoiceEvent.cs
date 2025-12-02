using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class InvoiceEvent
{
    public long Gkey { get; set; }

    public string? Container { get; set; }

    public DateTime? Date { get; set; }

    public string? Invoice { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Invoiced { get; set; }

    public string FE { get; set; } = null!;

    public decimal? Teu { get; set; }

    public string? Cat { get; set; }

    public string? Imo { get; set; }

    public string Rf { get; set; } = null!;

    public string Oog { get; set; } = null!;

    public string? Tariff { get; set; }

    public string? Event { get; set; }

    public string? DescriptionEn { get; set; }

    public string? DescryptionHr { get; set; }

    public string? VesselId { get; set; }

    public string? VesselName { get; set; }

    public string? VeselVisitId { get; set; }

    public string? LineOperatorId { get; set; }

    public string? Measure { get; set; }

    public string? Status { get; set; }

    public string? User { get; set; }

    public int? Item { get; set; }

    public string? Table { get; set; }

    public string? PayeeCustomerId { get; set; }

    public string? Role { get; set; }

    public int Id { get; set; }

    public decimal? Percentage { get; set; }
}
