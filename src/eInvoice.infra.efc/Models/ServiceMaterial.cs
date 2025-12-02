using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class ServiceMaterial
{
    public int Id { get; set; }

    public string Material { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? Createdby { get; set; }

    public DateTime? LastChange { get; set; }

    public string? ChangedBy { get; set; }

    public string? MaterialType { get; set; }

    public string? IndustrySector { get; set; }

    public string? MaterialGroup { get; set; }

    public string? BaseUnitOfMeasure { get; set; }

    public string? Division { get; set; }

    public string? GenItemCatGrp { get; set; }

    public string? MaterialDescription { get; set; }

    public string? SalesOrganization { get; set; }

    public string? DistributionChannel { get; set; }

    public string? ItemCategoryGroup { get; set; }

    public string? AcctAssignmentGrp { get; set; }

    public string? SalesUnit { get; set; }

    public string? TaxClass { get; set; }

    public string? Plant { get; set; }

    public string? ProfitCenter { get; set; }

    public string? AlternativeUnitOfMeasure { get; set; }

    public int? Numerator { get; set; }

    public int? Denominator { get; set; }

    public string? OldMaterialNumber { get; set; }

    public string? RecordStatus { get; set; }

    public DateTime? Updated { get; set; }
}
