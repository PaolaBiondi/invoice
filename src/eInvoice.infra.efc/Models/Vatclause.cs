using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

[Table("VATclause")]
public partial class Vatclause
{
    public int Id { get; set; }

    public decimal? Tax { get; set; }

    public string? Description { get; set; }

    public string? InvoiceTextAlt { get; set; }

    public string? InvoiceTextLoc { get; set; }

    public DateTime? Changed { get; set; }

    public string? User { get; set; }

    public bool? Valid { get; set; }

    public string? CustomerNumber { get; set; }
}
