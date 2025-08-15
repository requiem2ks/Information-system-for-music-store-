using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ReservationDTO
{
    public DateOnly Reservationdate { get; set; }

    public DateOnly? Reservationenddate { get; set; }
}
