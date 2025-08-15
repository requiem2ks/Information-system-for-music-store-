using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Provider
{
    public int Providerid { get; set; }

    public string Nameprovider { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
