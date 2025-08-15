using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class OrderlineDTO
{
    public int Orderid { get; set; }

    public int Productid { get; set; }

    public int Settingid { get; set; }

    public int? Discount { get; set; }

    public int Quantity { get; set; }

    public decimal Unitprice { get; set; }
}
