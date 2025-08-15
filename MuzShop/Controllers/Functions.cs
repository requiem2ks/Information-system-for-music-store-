using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;


namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopFunctionsController : ControllerBase
    {
        private readonly DatabaseFunctionsService _service;

        public ShopFunctionsController(DatabaseFunctionsService service)
        {
            _service = service;
        }
        


        [HttpGet("products")]

        public async Task<IActionResult> GetProductsByCategoryAndAvailability(
            [FromQuery] string categoryname,
            [FromQuery] bool onlyAvailable)
        {
            try
            {
                var products = await _service.GetProductsByCategoryAndAvailability(categoryname, onlyAvailable);
                return products.Count == 0 ? NotFound() : Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("orders-history")]
        public async Task<IActionResult> GetOrderHistoryByCustomer(
        [FromQuery] int clientId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
        {
            try
            {
                // Получаем заказы клиента с фильтрацией по дате
                var query = _service.GetOrdersQueryable()
                    .Where(o => o.Clientid == clientId &&
                               (!fromDate.HasValue || o.Orderdate >= fromDate) &&
                               (!toDate.HasValue || o.Orderdate <= toDate))
                    .Include(o => o.Orderlines)
                        .ThenInclude(ol => ol.Product)
                    .Include(o => o.StatusoforderNavigation) // Добавлено для статуса заказа
                    .Include(o => o.Payment) // Добавлено для платежа
                    .Include(o => o.Deliveries); // Добавлено для доставки

                var orders = await query.ToListAsync();

                // Формируем структуру ответа
                var result = orders.Select(o => new OrderHistoryDto
                {
                    OrderId = o.Orderid,
                    OrderDate = o.Orderdate,
                    OrderStatus = o.Statusoforder,
                    PaymentMethod = o.Payment?.Paymentmethod,
                    PaymentSum = o.Payment?.Paymentsum,
                    DeliveryStatus = o.Deliveries.FirstOrDefault()?.Statusofdelivery,
                    DeliveryDate = o.Deliveries.FirstOrDefault()?.Dateofdelivery,
                    OrderLines = o.Orderlines.Select(ol => new OrderLineDto
                    {
                        ProductId = ol.Productid,
                        ProductName = ol.Product?.Productname,
                        Quantity = ol.Quantity,
                        UnitPrice = ol.Unitprice,
                        Discount = ol.Discount
                    }).ToList()
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("sales-report")]

        public async Task<IActionResult> GetSalesReportWithProfit(
            [FromQuery] DateOnly fromDate,
            [FromQuery] DateOnly toDate)
        {
            try
            {
                var report = await _service.GetSalesReportWithProfit(fromDate, toDate);
                return report.Count == 0 ? NotFound() : Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
