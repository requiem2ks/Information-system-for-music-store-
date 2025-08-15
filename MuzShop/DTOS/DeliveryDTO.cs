using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class DeliveryDTO
{
    public int Orderid { get; set; }

    public int Courierid { get; set; }

    public string Deliveryaddress { get; set; } = null!;

    public int? Statusofdelivery { get; set; }

    public DateOnly? Dateofdelivery { get; set; }

}
