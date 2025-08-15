using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ReviewDTO
{
    public int Clientid { get; set; }

    public int Productid { get; set; }

    public string? Rewievtext { get; set; }

    public int Grade { get; set; }

    public DateOnly Dateofrevocation { get; set; }
}
