using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eInvoice.infra.efc.Infra.Models;

public partial class InvoiceItem
{
    public int Id { get; set; }

    public string? AltInvoiceNumber { get; set; }
    public SapInvoice? SapInvoice { get; set; }

    public short? Item { get; set; }

    public string? ServiceMaterial { get; set; }

    public decimal? BilledQty { get; set; }

    public string? SalesUnit { get; set; }

    public decimal? BilledQtySku { get; set; }

    public DateTime? PricingDate { get; set; }

    public decimal? BasicPrice { get; set; }

    public decimal? Price { get; set; }

    public decimal? Tax { get; set; }

    public decimal? OutputTax { get; set; }

    public string? Tariff { get; set; }

    public string? Event { get; set; }

    public decimal? Ratio { get; set; }

    public string? CancelledAltNumber { get; set; }

    public decimal? BilledQty1 { get; set; }

    public string? SalesUnit1 { get; set; }

    public string? ContractId { get; set; }

    public string? TaxClass { get; set; }

    public DateTime? ValidDate { get; set; }

    public string? Currency { get; set; }

    public string? ConditionType { get; set; }

    public string? PayTaxClass { get; set; }

    public string? PayCurrency { get; set; }

    public decimal? PayRatio { get; set; }

    public decimal? PayBasicPrice { get; set; }

    public decimal? PayPrice { get; set; }

    public decimal? PayTax { get; set; }

    public decimal? Percentage { get; set; }

}
