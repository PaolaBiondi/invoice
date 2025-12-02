using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class InvoiceStorage
{
    public int? Item { get; set; }

    public string? Container { get; set; }

    public string? Invoice { get; set; }

    public string FE { get; set; } = null!;

    public string? Cat { get; set; }

    public string? Carr { get; set; }

    public string? Imo { get; set; }

    public string Rf { get; set; } = null!;

    public string Oog { get; set; } = null!;

    public DateTime? StorageTime { get; set; }

    public DateTime? JobTime { get; set; }

    public short? Days { get; set; }

    public int? Free { get; set; }

    public decimal? Teu { get; set; }

    public string? TariffDays1 { get; set; }

    public short? Days1 { get; set; }

    public int TotDays1 { get; set; }

    public string? TariffDays2 { get; set; }

    public short? Days2 { get; set; }

    public int TotDays2 { get; set; }

    public string? TariffDays3 { get; set; }

    public short? Days3 { get; set; }

    public int TotDays3 { get; set; }

    public string? TariffDays4 { get; set; }

    public short? Days4 { get; set; }

    public int TotDays4 { get; set; }

    public string? Tariff { get; set; }

    public int? OnTerminal { get; set; }

    public string? Event { get; set; }

    public string? DescriptionEn { get; set; }

    public string? DescriptionHr { get; set; }

    public string? LineOperatorId { get; set; }

    public string? VesselId { get; set; }

    public string? VesselName { get; set; }

    public string? VeselVisitId { get; set; }

    public string? Measure { get; set; }

    public long Gkey { get; set; }

    public string? Status { get; set; }

    public string? User { get; set; }

    public string? Table { get; set; }

    public DateTime? Invoiced { get; set; }

    public DateTime? Created { get; set; }

    public string? PayeeCustomerId { get; set; }

    public string? Role { get; set; }

    public string? ContractId { get; set; }

    public int Id { get; set; }

    public DateTime? Eesgenerated { get; set; }
}
