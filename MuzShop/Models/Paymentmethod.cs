using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Paymentmethod
{
    public int Paymentmethodid { get; set; }

    public string Paymentmethodname { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
