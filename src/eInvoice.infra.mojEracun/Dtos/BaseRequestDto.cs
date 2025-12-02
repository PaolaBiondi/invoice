namespace eInvoice.infra.mojEracun.Dtos
{
    public abstract class BaseRequestDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string CompanyId { get; set; } = null!;
        public string SoftwareId { get; set; } = null!;

        public static T Build<T>(EinvoiceSettings settings) where T : BaseRequestDto, new()
        {
            T dto = new T
            {
                Username = settings.Username,
                Password = settings.Password,
                CompanyId = settings.CompanyId,
                SoftwareId = settings.SoftwareId
            };
            return dto;
        }
    }
}
