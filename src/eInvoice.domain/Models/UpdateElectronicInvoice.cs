using eInvoice.domain.Common;

namespace eInvoice.domain.Models
{
    public class UpdateElectronicInvoice
    {
        public string InvoiceNumber { get; set; } = null!;
        public byte[]? Ogds { get; set; }
        public int ElectronicId { get; set; }
        public DateTime Created { get; set; }
        public int StatusId { get; set; }
        public DateTime Sent { get; set; }
    }
}
