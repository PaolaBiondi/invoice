namespace eInvoice.infra.efc.Models;

public class  Configuration
{
    public int Id { get; set; }
    public string ConfigName { get; set; } = null!;
    public string ConfigValue { get; set; } = null!;
}
