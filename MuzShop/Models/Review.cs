using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Review
{
    public int Reviewid { get; set; }

    public int Clientid { get; set; }

    public int Productid { get; set; }

    public string? Rewievtext { get; set; }

    public int Grade { get; set; }

    public DateOnly Dateofrevocation { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Grade GradeNavigation { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
