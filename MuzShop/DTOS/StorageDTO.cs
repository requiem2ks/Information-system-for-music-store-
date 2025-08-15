using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class StorageDTO
{
    public int Productid { get; set; }

    public int Purchaseid { get; set; }

    public int? Quantity { get; set; }
}
