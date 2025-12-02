using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

[Table("InvoiceAdded")]
public partial class InvoiceAdded
{
    public int Id { get; set; }

    public string? InvoiceNumber { get; set; }

    public int? VatClauseId { get; set; }

    public byte[]? SnapshotPdf { get; set; }

    public DateTime? LastChange { get; set; }

    public byte[]? SnapshotAtt { get; set; }

    public string? SendingStatus { get; set; }

    public DateTime? SendingTime { get; set; }

    public byte[]? InvoiceMessage { get; set; }

    public DateTime? DownloadTime { get; set; }

    public int? InvoiceId { get; set; }

    public short? StatusId { get; set; }

    public DateTime? Sent { get; set; }

    public DateTime? Modified { get; set; }

    public DateTime? Delivered { get; set; }
}
