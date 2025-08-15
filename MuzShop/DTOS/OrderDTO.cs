using System;
using System.Collections.Generic;

namespace MuzShop.Models;

public partial class OrderDTO
{
    public DateOnly Orderdate { get; set; }

    public DateOnly? Orderenddate { get; set; }

    public int? Clientid { get; set; }

    public int Paymentid { get; set; }

    public int Employeeid { get; set; }

    public int? Statusoforder { get; set; }
}

public class OrderCreationDTO
{
    public DateOnly? Orderenddate { get; set; }
    public int Paymentid { get; set; }
    public ClientCreationDTO Client { get; set; }
    public List<OrderlineCreationDTO> Items { get; set; }
    public bool NeedReservation { get; set; } = true;
}

public class ClientCreationDTO
{
    public int Clientid { get; set; }
    public string Fio { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
}

public class OrderlineCreationDTO
{
    public int Productid { get; set; }
    public int Settingid { get; set; }
    public decimal? Discount { get; set; }
    public int Quantity { get; set; }
    public decimal Unitprice { get; set; }
}

public class OrderFullResponse
{
    public int OrderId { get; set; }
    public int PaymentId { get; set; }
    public List<int> ReservationIds { get; set; } = new List<int>();
    public decimal TotalAmount { get; set; }
}