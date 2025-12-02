using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eInvoice.infra.efc.Infra.Models;

[Table("BusinessSpaceArea")]
public partial class BusinessSpaceArea
{
    public int Id { get; set; }

    public string? BusinessSpaceName { get; set; }

    public string? BusinessSpaceCode { get; set; }
}
