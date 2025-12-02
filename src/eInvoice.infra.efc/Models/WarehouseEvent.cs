using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class WarehouseEvent
{
    public int Id { get; set; }

    public string? EventType { get; set; }

    public string? CardId { get; set; }

    public string? DocumentNo { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal? Quantity1 { get; set; }

    public string? Unit1 { get; set; }

    public DateTime? DateIn { get; set; }

    public DateTime? DateJob { get; set; }

    public int? StorageDays { get; set; }

    public int? FreeDays { get; set; }

    public int? Tariffdays { get; set; }

    public string? Tariff { get; set; }

    public string? Status { get; set; }

    public string? User { get; set; }

    public DateTime? Billed { get; set; }

    public string? Invoice { get; set; }

    public string? PayeeCustomerId { get; set; }

    public string? ContractId { get; set; }

    public string? Category { get; set; }

    public string? ImdgClass { get; set; }

    public short? BillingMethod { get; set; }

    public decimal? Weight { get; set; }

    public string? Mrn { get; set; }

    public DateTime? DateMrn { get; set; }

    public string? TransportId { get; set; }

    public string? Loctype { get; set; }

    public string? OrderNo { get; set; }

    public decimal? QuantityOrder { get; set; }

    public decimal? Volume { get; set; }

    public string? InputOrderNo { get; set; }

    public string? OutputDocumentNo { get; set; }

    public string? EventNote { get; set; }

    public decimal? CurrentQuantity { get; set; }

    public int? LinkedEventId { get; set; }

    public string? Container { get; set; }

    public int? Cegkey { get; set; }
}
