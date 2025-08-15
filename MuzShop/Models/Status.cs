using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Status
{
    public int Statusid { get; set; }

    public string Namestatus { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
