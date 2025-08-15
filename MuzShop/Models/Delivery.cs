using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Delivery
{
    public int Deliveryid { get; set; }

    public int Orderid { get; set; }

    public int Courierid { get; set; }

    public string Deliveryaddress { get; set; } = null!;

    public int? Statusofdelivery { get; set; }

    public DateOnly? Dateofdelivery { get; set; }

    public virtual Courier Courier { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Status? StatusofdeliveryNavigation { get; set; }
}
