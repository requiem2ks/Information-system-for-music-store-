using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Payment
{
    public int Paymentid { get; set; }

    public decimal? Paymentsum { get; set; }

    public DateOnly? Dateofpayment { get; set; }

    public int? Paymentmethod { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Paymentmethod? PaymentmethodNavigation { get; set; }
}
