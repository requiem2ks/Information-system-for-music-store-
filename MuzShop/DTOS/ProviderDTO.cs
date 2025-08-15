using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ProviderDTO
{
    public string Nameprovider { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }
}
