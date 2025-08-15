using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MuzShop.Models;

namespace MuzShop.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly PostgresContext _db;

        public OrderController(PostgresContext context)
        {
            _db = context;
        }

        // Получение заказов текущего пользователя
        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized("Не удалось определить пользователя");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("Некорректный идентификатор пользователя");

            var user = await _db.Users
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Userid == userId);

            if (user == null)
                return NotFound("Пользователь не найден");

            if (user.Client == null)
                return BadRequest("Только клиенты могут просматривать заказы");

            var orders = await _db.Orders
                .Where(o => o.Clientid == user.Client.Clientid)
                .Include(o => o.Payment)
                .Include(o => o.Employee)
                .OrderByDescending(o => o.Orderdate)
                .ToListAsync();

            return Ok(orders);
        }


        // Получение всех заказов (только для админов и сотрудников)
        [HttpGet]
        [Authorize(Roles = "admin,employee")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return await _db.Orders.ToListAsync();
        }

        // Получение конкретного заказа
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _db.Users
                .Include(u => u.Client)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Userid == userId);

            // Проверка прав доступа
            if (user.Role.Rolename != "admin" &&
                user.Role.Rolename != "employee" &&
                user.Client?.Clientid != order.Clientid)
            {
                return Forbid();
            }

            return order;
        }

        // Создание нового заказа
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderCreationDTO orderCreationDto)
        {

            // 1. Get current user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _db.Users
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Userid == userId);

            if (user == null)
                return Unauthorized("User not found");

            // 2. Update or create client information
            if (user.Client == null)
            {
                // Create new client if doesn't exist
                user.Client = new Client
                {
                    Fio = orderCreationDto.Client.Fio,
                    Phone = orderCreationDto.Client.Phone,
                    Address = orderCreationDto.Client.Address
                };
            }
            else
            {
                // Update existing client
                user.Client.Fio = orderCreationDto.Client.Fio;
                user.Client.Phone = orderCreationDto.Client.Phone;
                user.Client.Address = orderCreationDto.Client.Address;
            }

            // 3. Create the order
            var order = new Order
            {
                Orderdate = DateOnly.FromDateTime(DateTime.UtcNow), 
                Orderenddate = orderCreationDto.Orderenddate,
                Clientid = user.Client.Clientid,
                Paymentid = orderCreationDto.Paymentid,
                Statusoforder = 1, // pending
                Employeeid = 1 // Default employee ID
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // 4. Add order items
            foreach (var item in orderCreationDto.Items)
            {
                var orderItem = new Orderline
                {
                    Orderid = order.Orderid,
                    Productid = item.Productid,
                    Quantity = item.Quantity,
                    Unitprice = item.Unitprice,
                    Settingid = item.Settingid,
                };
                _db.Orderlines.Add(orderItem);
            }

            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Orderid }, order);
        }

        
        // Обновление заказа
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,employee")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO orderDto)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Orderenddate = orderDto.Orderenddate;
            order.Paymentid = orderDto.Paymentid;
            order.Employeeid = orderDto.Employeeid;
            order.Statusoforder = orderDto.Statusoforder;

            _db.Orders.Update(order);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Удаление заказа
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}