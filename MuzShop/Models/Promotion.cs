using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Promotion
{
    public int Stockid { get; set; }

    public string Namestock { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public string Typeofstock { get; set; } = null!;

    public decimal Discountvalue { get; set; }

    public virtual ICollection<Promotionproduct> Promotionproducts { get; set; } = new List<Promotionproduct>();
}
