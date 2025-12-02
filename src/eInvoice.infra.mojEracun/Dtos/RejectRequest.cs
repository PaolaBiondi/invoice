using System.Text.Json.Serialization;

namespace eInvoice.infra.mojEracun.Dtos
{
    public class RejectRequest: BaseRequestDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InternalMark { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateOnly? IssueDate { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SenderIdentifierValue { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RecipientIdentifierValue { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ElectronicId { get; set; }
        public DateOnly RejectionDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string RejectionReasonType { get; set; } = "O";
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RejectionReasonDescription { get; set; } = "Greška u dostavljenom računu";
    }
}
