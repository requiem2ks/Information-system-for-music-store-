using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly PostgresContext db;

        public ProductController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            return await db.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Productid == id);
            if (product == null)
                return NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] ProductDTO productDto)
        {
            if (productDto == null)
            {
                return BadRequest();
            }

            var product = new Product
            {
                Productname = productDto.Productname,
                Description = productDto.Description,
                Price = productDto.Price,
                Productcategoryid = productDto.Productcategoryid
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = product.Productid }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] ProductDTO productDto)
        {
            if (productDto == null)
            {
                return BadRequest();
            }

            var existingProduct = await db.Products.FirstOrDefaultAsync(x => x.Productid == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Productname = productDto.Productname;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.Productcategoryid = productDto.Productcategoryid;

            db.Products.Update(existingProduct);
            await db.SaveChangesAsync();
            return Ok(existingProduct);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Productid == id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpGet("withcategories")]
        public async Task<IActionResult> GetWithCategories()
        {
            try
            {
                var products = await db.Products
                    .Include(p => p.Productcategory) // Теперь должно работать без неоднозначности
                    .Select(p => new
                    {
                        Product = new
                        {
                            p.Productid,
                            p.Productname,
                            p.Description,
                            p.Price,
                            p.Productcategoryid,
                            p.image
                        },
                        Category = new
                        {
                            Namecategory = p.Productcategory.Namecategory,
                            Descriptioncategory = p.Productcategory.Descriptioncategory
                        }
                    })
                    .AsNoTracking() // Рекомендуется для read-only операций
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // Метод для резервирования (используем анонимные объекты)
        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveProduct([FromBody] ReservationRequestDto request)
        {
            try
            {
                // 1. Создаем заказ для резервации (только обязательные поля)
                // Временное решение (не рекомендуется для production)
                var order = new Order
                {
                    Orderdate = DateOnly.FromDateTime(DateTime.Now),
                    Clientid = request.UserId,
                    Paymentid = 0, // Временное значение
                    Employeeid = 0,
                    Statusoforder = 0
                };

                // Затем после сохранения обновите эти поля:
                order.Paymentid = 0;
                await db.SaveChangesAsync();

                db.Orders.Add(order);
                await db.SaveChangesAsync();

                // 2. Создаем резервацию
                var reservation = new Reservation
                {
                    Reservationdate = DateOnly.FromDateTime(DateTime.Now),
                    Reservationenddate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
                };

                db.Reservations.Add(reservation);
                await   db.SaveChangesAsync();

                // 3. Добавляем товар в заказ (orderline)
                var product = await db.Products.FindAsync(request.ProductId);
                if (product == null)
                    return NotFound("Товар не найден");

                var orderLine = new Orderline
                {
                    Orderid = order.Orderid,
                    Productid = request.ProductId,
                    Quantity = request.Quantity,
                    Unitprice = product.Price ?? 0,
                    // Оставляем null (если Settingid допускает null)
                    Settingid = 0,
                    Discount = null
                };

                db.Orderlines.Add(orderLine);
                await db.SaveChangesAsync();

                // 4. Создаем запись о резервировании товара
                var productReservation = new Productreservation
                {
                    Reservationid = reservation.Reservationid,
                    Orderid = order.Orderid,
                    Productid = request.ProductId,
                    Quantityofreserved = request.Quantity,
                    Unitpricereservation = product.Price ?? 0,
                    // Предоплата может быть null (если процент не фиксированный)
                    Prepaymentpercentage = null
                };

                db.Productreservations.Add(productReservation);
                await db.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    OrderId = order.Orderid,
                    ReservationId = reservation.Reservationid,
                    EndDate = reservation.Reservationenddate?.ToString("dd.MM.yyyy")
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
