using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class PersonaldiscountDTO
{
    public int Clientid { get; set; }

    public decimal Discountvalue { get; set; }

    public DateOnly? Startdate { get; set; }

    public DateOnly? Enddate { get; set; }
}
