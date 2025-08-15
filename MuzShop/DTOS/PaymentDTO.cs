using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class PaymentDTO
{
    public decimal Paymentsum { get; set; }

    public DateOnly? Dateofpayment { get; set; }

    public int? Paymentmethod { get; set; }
}
