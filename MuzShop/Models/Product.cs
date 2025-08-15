using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Product
{
    public int Productid { get; set; }

    public string? Productname { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int Productcategoryid { get; set; }

    public string? image { get; set; }


    public virtual ICollection<Instrumentsetup> Instrumentsetups { get; set; } = new List<Instrumentsetup>();

    public virtual ICollection<Orderline> Orderlines { get; set; } = new List<Orderline>();

    public virtual Productcategory Productcategory { get; set; } = null!;

    public virtual ICollection<Productreservation> Productreservations { get; set; } = new List<Productreservation>();

    public virtual ICollection<Promotionproduct> Promotionproducts { get; set; } = new List<Promotionproduct>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Shoppingcart> Shoppingcarts { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
