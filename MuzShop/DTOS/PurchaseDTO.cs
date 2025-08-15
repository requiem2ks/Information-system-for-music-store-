using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class PurchaseDTO
{
    public int Providerid { get; set; }

    public int Productid { get; set; }

    public decimal Unitprice { get; set; }

    public int Quantity { get; set; }

    public DateOnly Datepurchase { get; set; }
}
