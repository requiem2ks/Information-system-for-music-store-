using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ReceiptsatthestorageDTO
{
    public int Purchaseid { get; set; }

    public int Storageid { get; set; }

    public DateOnly Dateofreceipt { get; set; }
}
