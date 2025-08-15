using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Receiptsatthestorage
{
    public int Receiptsatthestoragesid { get; set; }

    public int Purchaseid { get; set; }

    public int Storageid { get; set; }

    public DateOnly Dateofreceipt { get; set; }

    public virtual Purchase Purchase { get; set; } = null!;

    public virtual Storage Storage { get; set; } = null!;
}
