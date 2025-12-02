using System;
using System.Collections.Generic;

namespace eInvoice.infra.efc.Infra.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? UserPassword { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool? UserActiv { get; set; }

    public DateTime? UserCreated { get; set; }

    public string? UserCreator { get; set; }
    public User? UserCreatorNavigation { get; set; }

    public DateTime? UserChanged { get; set; }

    public string? UserChanger { get; set; }
    public User? UserChangerNavigation { get; set; }
}
