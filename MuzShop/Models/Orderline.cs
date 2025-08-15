using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Orderline
{
    public int Orderlineid { get; set; }

    public int Orderid { get; set; }

    public int Productid { get; set; }

    public int Settingid { get; set; }

    public int? Discount { get; set; }

    public int Quantity { get; set; }

    public decimal Unitprice { get; set; }

    public virtual Personaldiscount? DiscountNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Instrumentsetup Setting { get; set; } = null!;
}
