using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Productcategory
{
    public int Productcategoryid { get; set; }

    public string? Namecategory { get; set; }

    public string? Descriptioncategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
