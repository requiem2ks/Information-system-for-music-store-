using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Productreservation
{
    public int Productreservationid { get; set; }

    public int Reservationid { get; set; }

    public int Orderid { get; set; }

    public int Productid { get; set; }

    public int Quantityofreserved { get; set; }

    public decimal Unitpricereservation { get; set; }

    public int? Prepaymentpercentage { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Reservation Reservation { get; set; } = null!;
}
