using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Order
{
    public int Orderid { get; set; }

    public DateOnly Orderdate { get; set; }

    public DateOnly? Orderenddate { get; set; }

    public int Clientid { get; set; }

    public int Paymentid { get; set; }

    public int Employeeid { get; set; }

    public int? Statusoforder { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Orderline> Orderlines { get; set; } = new List<Orderline>();

    public virtual Payment Payment { get; set; } = null!;

    public virtual ICollection<Productreservation> Productreservations { get; set; } = new List<Productreservation>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual Status? StatusoforderNavigation { get; set; }
}
