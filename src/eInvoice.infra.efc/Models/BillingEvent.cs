using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class BillingEvent
{
    public long Gkey { get; set; }

    public string? DescriptionHr { get; set; }

    public string? DescriptionEn { get; set; }

    public string? EventType { get; set; }

    public string? Tariff { get; set; }

    public string? TariffDays1 { get; set; }

    public string? TariffDays2 { get; set; }

    public string? TariffDays3 { get; set; }

    public string? TariffDays4 { get; set; }

    public string? Container { get; set; }

    public string? EqLength { get; set; }

    public string FulEmpty { get; set; } = null!;

    public string? LineOperatorId { get; set; }

    public string? Category { get; set; }

    public bool IsOog { get; set; }

    public bool IsRefrigerated { get; set; }

    public bool IsHazardous { get; set; }

    public string? ImdgClass { get; set; }

    public string? Status { get; set; }

    public string? User { get; set; }

    public DateTime? Billed { get; set; }

    public DateTime? Changed { get; set; }

    public string? Vessel { get; set; }

    public string? VeselVisitId { get; set; }

    public DateTime? VesselTime { get; set; }

    public string? VesselId { get; set; }

    public DateTime? JobTime { get; set; }

    public short? Days1 { get; set; }

    public short? Days2 { get; set; }

    public short? Days3 { get; set; }

    public short? Days4 { get; set; }

    public DateTime? StorageTime { get; set; }

    public string? MidVessel { get; set; }

    public DateTime? MidVesselTime { get; set; }

    public int? Quantity { get; set; }

    public string? IbLoctype { get; set; }

    public string? ObLoctype { get; set; }

    public int? FreeDays { get; set; }

    public int? FreeExtra { get; set; }

    public int? FreeBert { get; set; }

    public string? Loctype { get; set; }

    public string? IntVessel { get; set; }

    public string? IntVeselVisitId { get; set; }

    public DateTime? IntVesselTime { get; set; }

    public string? IntVesselId { get; set; }

    public long? SourceEventGkey { get; set; }

    public DateTime? VvAta { get; set; }

    public DateTime? VvAtd { get; set; }

    public short? BillingMethod { get; set; }

    public short? Days { get; set; }

    public long? UfvGkey { get; set; }

    public string? Measure { get; set; }

    public string? Invoice { get; set; }

    public string? PayeeCustomerId { get; set; }

    public string? Role { get; set; }

    public DateTime? EventStartTime { get; set; }

    public DateTime? EventEndTime { get; set; }

    public bool? ReceiveInStorage { get; set; }

    public bool? DeliverInStorage { get; set; }

    public short? LoadInStorage { get; set; }

    public short? DischInStorage { get; set; }

    public string? ContractId { get; set; }

    public string? FwdRefNbr { get; set; }
}
