using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ProductDTO
{
    public string? Productname { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int Productcategoryid { get; set; }

    public string? image { get; set; }
}
