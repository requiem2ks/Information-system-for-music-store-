using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ProductreservationDTO
{
    public int Reservationid { get; set; }

    public int Orderid { get; set; }

    public int Productid { get; set; }

    public int Quantityofreserved { get; set; }

    public decimal Unitpricereservation { get; set; }

    public int? Prepaymentpercentage { get; set; }
}
