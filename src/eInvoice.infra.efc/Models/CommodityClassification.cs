using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace eInvoice.infra.efc.Infra.Models;

public class CommodityClassification
{
    [Key]
    public int Id { get; set; }
    public string ClassificationCode { get; set; } = null!;
    [MaxLength(10)]
    [Unicode(false)]
    public string Tariff { get; set; } = null!;
    [MaxLength(50)]
    public string Event { get; set; } = null!;
    public Tariff? TariffNavigation { get; set; }
}
