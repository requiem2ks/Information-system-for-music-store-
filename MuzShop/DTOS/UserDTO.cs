using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class UserDTO
{
    public int? Roleid { get; set; }

    public int? Clientid { get; set; }

    public int? Employeeid { get; set; }

    public string Email { get; set; } = null!;

    public string Hashpassword { get; set; } = null!;

}
