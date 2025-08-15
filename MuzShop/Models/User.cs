using MuzShop.Models;
using System;
using System.Collections.Generic;

namespace MuzShop;

public partial class User
{
    public int Userid { get; set; }

    public int? Roleid { get; set; }

    public int? Clientid { get; set; }

    public int? Employeeid { get; set; }

    public string Email { get; set; } = null!;

    public string Hashpassword { get; set; } = null!;

    public virtual Client? Client { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Role? Role { get; set; }
}
