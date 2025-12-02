using System.ComponentModel.DataAnnotations;

namespace eInvoice.infra.mojEracun
{
    public class EinvoiceSettings
    {
        [Required, Url]
        public string EinvoiceServiceUri { get; set; } = string.Empty;
        [Required]
        public string ApiKey { get; set; } = string.Empty;
        [Required]
        public string CompanyId { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string SoftwareId { get; set; } = null!;
        public string? CompanyBu { get; set; }
        public string? MaskedEmail { get; set; }
        [Required]
        public string Ping { get; set; } = null!;
        [Required]
        public string MarkPaidWithoutElectronicId { get; set; } = null!;
        [Required]
        public string MarkPaidWithElectronicId { get; set; } = null!;
        [Required]
        public string RejectWithoutElectronicId { get; set; } = null!;
        [Required]
        public string RejectWithElectronicId { get; set; } = null!;
    }
}

