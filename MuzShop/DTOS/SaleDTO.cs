using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class SaleDTO
{
    public int Orderid { get; set; }

    public DateOnly? Saledate { get; set; }

    public decimal Totalamount { get; set; }

    public decimal Profitamount { get; set; }
}
