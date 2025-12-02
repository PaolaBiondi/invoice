namespace eInvoice.domain.Models
{
    public class CompanyDto
    {
        public string Name { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string TaxNumber { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? BuildingNumber { get; set; }
        public string AddressLine => $"{Street}, {PostalCode} {City}";
    }
}
