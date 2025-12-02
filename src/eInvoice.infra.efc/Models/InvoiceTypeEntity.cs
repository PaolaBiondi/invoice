using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

[Table("InvoiceType")]
public partial class InvoiceTypeEntity
{
    public int Id { get; set; }

    public string InvoiceTypeId { get; set; } = null!;

    public string? InvoiceDescription { get; set; }

    public string? Commentary { get; set; }

    public int? VatClause { get; set; }
    public Vatclause? VatClauseNavigation { get; set; }

    public string? SapbillingType { get; set; }

    public string? InvoiceDescriptionLocal { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
