using Microsoft.EntityFrameworkCore;
using MuzShop;
using MuzShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MuzShop.PostgresContext;
public class DatabaseFunctionsService
{
    private readonly PostgresContext _context;

    public DatabaseFunctionsService(PostgresContext context)
    {
        _context = context;
    }
    public IQueryable<Order> GetOrders()
    {
        return _context.Orders;
    }
    public IQueryable<Order> GetOrdersQueryable()
    {
        return _context.Orders.AsQueryable();
    }
    // 1. Получить список товаров по категории и наличию
    public async Task<List<ProductFDto>> GetProductsByCategoryAndAvailability(string categoryName, bool? inStock = null)
    {
        // В SQL передаем параметр, если nullable, нужно явно указывать
        return await _context.ProductsByCategoryResults
            .FromSqlRaw("SELECT * FROM get_products_by_category_and_availability({0}, {1})", categoryName, inStock)
            .ToListAsync();
    }

    // 2. Получить историю заказов клиента по id и диапазону дат
    public async Task<List<OrderHistoryDto>> GetOrderHistoryByCustomer(int clientId, DateOnly startDate, DateOnly endDate)
    {
        return await _context.OrderHistoryResults
            .FromSqlRaw("SELECT * FROM GetOrderHistoryByCustomer({0}, {1}, {2})", clientId, startDate, endDate)
            .ToListAsync();
    }

    // 3. Получить отчет по продажам за период
    public async Task<List<SalesReportDto>> GetSalesReportWithProfit(DateOnly startDate, DateOnly endDate)
    {
        return await _context.SalesReportResults
            .FromSqlRaw("SELECT * FROM GetSalesReportWithProfit({0}, {1})", startDate, endDate)
            .ToListAsync();
    }
}
