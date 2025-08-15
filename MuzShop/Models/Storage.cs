using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Storage
{
    public int Storageid { get; set; }

    public int Productid { get; set; }

    public int Purchaseid { get; set; }

    public int? Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Purchase Purchase { get; set; } = null!;

    public virtual ICollection<Receiptsatthestorage> Receiptsatthestorages { get; set; } = new List<Receiptsatthestorage>();
}
