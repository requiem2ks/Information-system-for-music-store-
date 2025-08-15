using System.ComponentModel.DataAnnotations;

public class SalesReportDto
{
    [Key]
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public long SoldQuantity { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Profit { get; set; }
    public long StorageQuantity { get; set; }
}