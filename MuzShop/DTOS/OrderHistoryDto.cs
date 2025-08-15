using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class OrderHistoryDto
{
    [Key]
    public int OrderId { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly? OrderEndDate { get; set; }
    public int? OrderStatus { get; set; }
    public decimal? PaymentSum { get; set; }
    public int? PaymentMethod { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public int? DeliveryStatus { get; set; }
    public DateOnly? DeliveryDate { get; set; }

    // Переименовано в OrderLines для соответствия модели
    public List<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();
    public List<OrderLineDto> Payment { get; set; } = new List<OrderLineDto>();
}

public class OrderLineDto
{
    [Key]
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
}