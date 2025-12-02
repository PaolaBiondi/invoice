using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

public partial class SapInvoice
{
    public int Id { get; set; }

    public string? InvoiceNumber { get; set; }

    public string? DistrChanel { get; set; }

    public string? Division { get; set; }

    public string AltInvoiceNumber { get; set; } = null!;
    
    public Invoice Invoice { get; set; } = null!;

    public DateTime? Extracted { get; set; }

    public string? ExtractStatus { get; set; }

    public string? Sapinvoice1 { get; set; }

    public DateTime Changed { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; } = new List<InvoiceItem>();
    [NotMapped]
    public decimal TotalPayAmount => InvoiceItems?.Sum(i => i.PayPrice) ?? 0;
    [NotMapped]
    public decimal TotalPayTax => InvoiceItems?.Sum(i => i.PayTax) ?? 0;
}
