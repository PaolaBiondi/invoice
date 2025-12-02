namespace eInvoice.infra.mojEracun.Dtos
{
    public class  CheckCompanyIdRequest: BaseRequestDto
    {
        public string IdentifierType { get; } = "0";
        public string IdentifierValue { get; set; } = null!;
    }
}
