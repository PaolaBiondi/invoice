using eInvoice.domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

[Table("InvoiceDetails")]
public class InvoiceDetail
{
    [Key]
    public int Id { get; set; }
    public int BusinessProcessProfileId { get; set; }
    [Unicode(false)]
    [MaxLength(30)]
    public string? InvoiceNumber { get; set; }
    public SendingStatusQualifier SendingStatus { get; set; } = SendingStatusQualifier.NotReady;
    
    // Mark as Paid
    public bool IsPaid { get; set; } = false;
    public string? PaidConfirmationMessage { get; set; }
    public DateTimeOffset? PaidFiscalizationDateTime { get; set; }
    
    // Mark as Rejected
    public bool IsRejected { get; set; } = false;
    public string? RejectedConfirmationMessage { get; set; }
    public DateTimeOffset? RejectedFiscalizationDateTime { get; set; }
    
    public DateTimeOffset Created   { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? Updated { get; set; }

    // Navigations
    public Invoice InvoiceNavigation    { get; set; } = null!;
    public BusinessProcessProfile BusinessProcessProfileNavigation { get; set; } = null!;
}
