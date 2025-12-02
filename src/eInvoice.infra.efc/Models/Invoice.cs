using eInvoice.domain.Common;

namespace eInvoice.infra.efc.Infra.Models;

public class Invoice
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public string? CancelledBillingNumber { get; set; }

    public BillingMethodQualifier? BillingMethod { get; set; }

    public string? BillOfLading { get; set; }


    public string? InvoiceType { get; set; }
    public InvoiceTypeEntity? InvoiceTypeNavigation { get; set; }

    public DateTime? BillingDate { get; set; }

    public DateTime? BaselineDate { get; set; }

    public string? Currency { get; set; }

    public string? SalesGroup { get; set; }

    public string? SalesDistrict { get; set; }

    public string? SoldToParty { get; set; }

    public string? Payer { get; set; }

    public string? BillToParty { get; set; }

    public string Company { get; set; } = null!;

    public string? ShipToParty { get; set; }

    public string? Consignee { get; set; }

    public string? Ponumber { get; set; }

    public string? TermsOfPaymentKey { get; set; }

    public string? SapbillingType { get; set; }


    public string? Reference { get; set; }

    public DateTime? DatePeriodFrom { get; set; }

    public DateTime? DatePeriodTo { get; set; }

    public DateTime? Invoiced { get; set; }

    public string? InvoicedUser { get; set; }
    public User? InvoicedUserNavigation { get; set; }
    public DateTime? Controlled { get; set; }

    public string? ControlledUser { get; set; }

    public DateTime? Approved { get; set; }

    public string? ApprovedUser { get; set; }

    public string? InvoiceStatus { get; set; }

    public DateTime? Extracted { get; set; }

    public string? ExtractStatus { get; set; }

    public string? Fiscalization { get; set; }

    public string? VesselName { get; set; }

    public string? VesseVisitlId { get; set; }


    public string? CustomerRefNbr { get; set; }

    public decimal? Ratio { get; set; }

    public string? Language { get; set; }

    public DateTime? Rejected { get; set; }

    public string? RejectedUser { get; set; }

    public string? TaxClass { get; set; }

    public ICollection<SapInvoice> SapInvoices { get; set; } = new List<SapInvoice>();
    public EdiinvoicingTo? EdiinvoicingTo { get; set; }
    public ICollection<InvoiceAttachment> InvoiceAttachments { get; set; } = new List<InvoiceAttachment>();
}
