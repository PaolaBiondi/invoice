namespace eInvoice.infra.mojEracun.Dtos
{
    public class InvoiceUploadResponse
    {
        public Dictionary<string, List<string>>? Errors { get; set; }
        public int Status { get; set; } = -1;
        public string? TraceId { get; set; } = null!;

        public int ElectronicId { get; set; }
        public string? DocumentNr { get; set; }
        public DateTime Created { get; set; }
        public int StatusId { get; set; }
        public DateTime Sent { get; set; }
    }
}
