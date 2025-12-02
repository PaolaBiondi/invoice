using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class InBillProcessPool
{
    public int? Item { get; set; }

    public int? Idpool { get; set; }

    public string? LineOperatorId { get; set; }

    public DateTime? Date { get; set; }

    public short? Containers20 { get; set; }

    public short? Containers40 { get; set; }

    public short? Containers45 { get; set; }

    public short? PoolTeus { get; set; }

    public short? TotalTeus { get; set; }

    public short? Storage { get; set; }

    public string? DescriptionEn { get; set; }

    public int Select { get; set; }

    public string? Invoice { get; set; }

    public string? Measure { get; set; }

    public string? Status { get; set; }

    public string? User { get; set; }

    public string? Tariff { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Invoiced { get; set; }

    public string? PayeeCustomerId { get; set; }

    public string? Role { get; set; }

    public string? InheritFrom { get; set; }

    public int? TariffTeus0 { get; set; }

    public int? TariffTeus1 { get; set; }

    public int? TariffTeus2 { get; set; }

    public int? TariffTeus3 { get; set; }

    public string? Tariff1 { get; set; }

    public string? Tariff2 { get; set; }

    public string? Tariff3 { get; set; }

    public int? DaysOverPool { get; set; }

    public bool? OldOption { get; set; }
}
