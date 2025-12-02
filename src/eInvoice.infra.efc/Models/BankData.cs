using eInvoice.infra.efc.Models;
using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class BankData
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public string? BankName { get; set; }

    public string? AccountId { get; set; }

    public string? Swiftid { get; set; }

    public string? Ibanid { get; set; }

    public bool IsValid { get; set; }

    public string? CompanyCode { get; set; }

    public virtual Company? CompanyCodeNavigation { get; set; }
}
