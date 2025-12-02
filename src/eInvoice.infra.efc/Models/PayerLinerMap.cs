using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class PayerLinerMap
{
    public int Id { get; set; }

    public string? LineOperator { get; set; }

    public string? SoldToParty { get; set; }

    public string? Payer { get; set; }

    public string? BillToParty { get; set; }

    public string? ShipToParty { get; set; }

    public string? Language { get; set; }

    public string? Currency { get; set; }

    public string Source { get; set; } = null!;

    public string TaxClass { get; set; } = null!;

    public bool InvoiceOverEmail { get; set; }

    public bool OneInvoiceOnEmail { get; set; }

    public string? EmailAddress { get; set; }
}
