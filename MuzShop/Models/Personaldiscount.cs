using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Personaldiscount
{
    public int Personaldiscountid { get; set; }

    public int Clientid { get; set; }

    public decimal Discountvalue { get; set; }

    public DateOnly? Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Orderline> Orderlines { get; set; } = new List<Orderline>();
}
