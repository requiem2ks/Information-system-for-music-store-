using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ClientDTO
{
    public string? Fio { get; set; }

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }

}
