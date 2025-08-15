using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class ReservationRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; } // ClientId из таблицы Users

}
