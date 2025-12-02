using eInvoice.domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

/// <summary>
/// Contains attachments related to an invoice, such as XLS files.
/// For now it serves for xls files, since legacy application original pdf and attachment pdf saves in the table <see cref="InvoiceAdded(string)"/>
/// This table is a great candidate for storing attachments in a more structured way in the future.
/// </summary>
[Table("InvoiceAttachments")]
public class InvoiceAttachment
{
    [Key]
    public int Id { get; set; }
    [MaxLength(30)]
    [Unicode(false)]
    public string InvoiceNumber { get; set; } = null!;
    public Invoice Invoice { get; set; } = null!;
    public MimeCodeQualifier MimeCode { get; set; }
    public byte[] AttachmentXls { get; set; } = null!;
    [MaxLength(255)]
    public string FileName { get; set; } = null!;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? Created { get; set; }
}
