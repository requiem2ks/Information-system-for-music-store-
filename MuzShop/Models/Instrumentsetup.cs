using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Instrumentsetup
{
    public int Settingid { get; set; }

    public int Productid { get; set; }

    public string Settingtype { get; set; } = null!;

    public string? Descriptionsetting { get; set; }

    public decimal Setupcost { get; set; }

    public virtual ICollection<Orderline> Orderlines { get; set; } = new List<Orderline>();

    public virtual Product Product { get; set; } = null!;
}
