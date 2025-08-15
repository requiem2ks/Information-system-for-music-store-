using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class Reservation
{
    public int Reservationid { get; set; }

    public DateOnly Reservationdate { get; set; }

    public DateOnly? Reservationenddate { get; set; }

    public virtual ICollection<Productreservation> Productreservations { get; set; } = new List<Productreservation>();
}
