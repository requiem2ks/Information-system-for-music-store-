using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Promotionproduct
{
    public int Promotionproductsid { get; set; }

    public int Stockid { get; set; }

    public int Productid { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Promotion Stock { get; set; } = null!;
}
