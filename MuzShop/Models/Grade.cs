using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Grade
{
    public int Gradeid { get; set; }

    public int Valuegrade { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
