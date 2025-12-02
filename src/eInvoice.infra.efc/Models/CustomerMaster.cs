using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

public partial class CustomerMaster
{
    public int Id { get; set; }

    public string? CompanyCode { get; set; }

    public string CustomerNumber { get; set; } = null!;

    public string? SalesOrganization { get; set; }

    public string DistributionChannel { get; set; } = null!;

    public string Division { get; set; } = null!;

    public string? CustomerAccountGroup { get; set; }

    public string? TitleText { get; set; }

    public string? Name1 { get; set; }

    public string? Name2 { get; set; }

    public string? Name3 { get; set; }

    public string? Name4 { get; set; }

    public string? SearchTerm1 { get; set; }

    public string? SearchTerm2 { get; set; }

    public string? Street2 { get; set; }

    public string? HouseNumber { get; set; }

    public string? CityPostalCode { get; set; }

    public string? City { get; set; }

    public string? CityCountryKey { get; set; }

    public string? Region { get; set; }

    public string? LanguageKey { get; set; }

    public string? FirstTelephone { get; set; }

    public string? FirstMobile { get; set; }

    public string? FirstFax { get; set; }

    public string? EmailAddress { get; set; }

    public string? TaxNumber1 { get; set; }

    public string? TaxNumber2 { get; set; }

    public string? CountryCode { get; set; }

    public string? VatregNo { get; set; }

    public string? NatPerson { get; set; }

    public string? IndustryKey { get; set; }

    public string CustomerClass { get; set; } = null!;

    public string? IndustryCode { get; set; }

    public string? ContactPerson { get; set; }

    public string? TermsOfPaymentKey { get; set; }

    public string? SalesDistrict { get; set; }

    public string? SalesOffice { get; set; }

    public string? SalesGroup { get; set; }

    public string CustomerGroup { get; set; } = null!;

    public string? CustomersCurrency { get; set; }

    public string? CreditControlArea { get; set; }

    public string? TaxClass { get; set; }

    public string SoldToParty { get; set; } = null!;

    public string Consignee { get; set; } = null!;

    public string BillToParty { get; set; } = null!;

    public string Payer { get; set; } = null!;

    public string ShipToParty { get; set; } = null!;

    public string? BankCountryKey { get; set; }

    public string? BankKey { get; set; }

    public string? BankAccount { get; set; }

    public string? ReconciliationAccount { get; set; }

    public string? PreviousAccountNumber { get; set; }

    public string? InterestIndicator { get; set; }

    public string? PaymentBlock { get; set; }

    public string? HouseBank { get; set; }

    public string? LockBox { get; set; }

    public string? DunningProcedure { get; set; }

    public string? DunningBlock { get; set; }

    public int? DunningLevel { get; set; }

    public string? WithholdingTaxCode { get; set; }

    public string? WithholdingTaxIndicator { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? IndividualLimit { get; set; }

    public string? Currency { get; set; }

    public decimal? CreditLimit { get; set; }

    public string RecordStatus { get; set; } = null!;
}
