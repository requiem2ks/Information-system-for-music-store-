using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Courier
{
    public int Courierid { get; set; }

    public string? Fio { get; set; }

    public string? Typeoftransport { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Liftingcapacity { get; set; }

    public string? Maximumsize { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
