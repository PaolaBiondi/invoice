using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eInvoice.infra.mojEracun.Scheme
{
    public record SendResponse: IResponse
    {
        [JsonIgnore]
        public int StatusCode { get; set; } = -1;
        public int Status { get; set; }
        public string Message { get; set; } = null!;
        public string? TraceId { get; set; }
        public int ElectronicId { get; set; }
        public string? DocumentNr { get; set; }
        public int DocumentTypeId { get; set; }
        public string? DocumentTypeName { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? RecipientBusinessNumber { get; set; }
        public string? RecipientBusinessUnit { get; set; }
        public string? RecipientBusinessName { get; set; }
        public string? Created { get; set; }
        public string? Sent { get; set; }
        public string? Modified { get; set; }
        public string? Delivered { get; set; }
        public File? File { get; set; }
    }

    public record File
    {
        public string Value { get; set; } = null!;
        public IReadOnlyList<string> Messages { get; set; } = new List<string>();
    }
}
