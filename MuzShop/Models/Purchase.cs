using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Purchase
{
    public int Purchaseid { get; set; }

    public int Providerid { get; set; }

    public int Productid { get; set; }

    public decimal Unitprice { get; set; }

    public int Quantity { get; set; }

    public DateOnly Datepurchase { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;

    public virtual ICollection<Receiptsatthestorage> Receiptsatthestorages { get; set; } = new List<Receiptsatthestorage>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
