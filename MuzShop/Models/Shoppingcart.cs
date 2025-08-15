using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Shoppingcart
{
    public int Basketitemid { get; set; }

    public int Productid { get; set; }

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;
}
