using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using MuzShop.Models;

namespace MuzShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "client")]
    public class ClientPortalController : ControllerBase
    {
        private readonly PostgresContext _context;

        public ClientPortalController(PostgresContext context)
        {
            _context = context;
        }

        private int GetClientId()
        {
            var clientClaim = User.FindFirst("ClientId");
            return clientClaim != null ? int.Parse(clientClaim.Value) : 0;
        }

        private int GetProductId()
        {
            var productClaim = User.FindFirst("Productid");
            return productClaim != null ? int.Parse(productClaim.Value) : 0;
        }

        [HttpGet("products")]
        public ActionResult<IEnumerable<ProductDTO>> GetProducts()
        {
            var products = _context.Products
                .Include(p => p.Productcategory)
                .Select(p => new ProductDTO     
                {
                    Productname = p.Productname,
                    Description = p.Description,
                    Price = p.Price,
                    Productcategoryid = p.Productcategoryid
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<ProductcategoryDTO>> GetCategories()
        {
            var categories = _context.Productcategories
                .Select(c => new ProductcategoryDTO
                {
                    Namecategory = c.Namecategory,
                    Descriptioncategory = c.Descriptioncategory
                })
                .ToList();

            return Ok(categories);
        }

        [HttpGet("reviews")]
        public ActionResult<IEnumerable<ReviewDTO>> GetReviews()
        {
            var reviews = _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.Client)
                .Select(r => new ReviewDTO
                {
                    Productid = r.Productid,
                    Clientid = r.Clientid,
                    Grade = r.Grade,
                    Rewievtext = r.Rewievtext,
                    Dateofrevocation = r.Dateofrevocation
                })
                .ToList();

            return Ok(reviews);
        }

        [HttpGet("promotions")]
        public ActionResult<IEnumerable<PromotionDTO>> GetPromotions()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var promotions = _context.Promotions
                .Where(p => p.Enddate >= today)
                .Select(p => new PromotionDTO
                {
                    Namestock = p.Namestock,
                    Description = p.Description,
                    Startdate = p.Startdate,
                    Enddate = p.Enddate,
                    Typeofstock = p.Typeofstock
                })
                .ToList();

            return Ok(promotions);
        }

        [HttpGet("personal-discounts")]
        public ActionResult<IEnumerable<PersonaldiscountDTO>> GetPersonalDiscounts()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            int clientId = GetClientId();
            var discounts = _context.Personaldiscounts
                .Where(pd => pd.Clientid == clientId &&
                            (!pd.Enddate.HasValue || pd.Enddate >= today))
                .Select(pd => new PersonaldiscountDTO
                {
                    Clientid = pd.Clientid,
                    Discountvalue = pd.Discountvalue,
                    Startdate = pd.Startdate,
                    Enddate = pd.Enddate
                })
                .ToList();

            return Ok(discounts);
        }

        [HttpGet("orders")]
        public ActionResult<IEnumerable<OrderDTO>> GetClientOrders()
        {
            int clientId = GetClientId();
            var orders = _context.Orders
                .Where(o => o.Clientid == clientId)
                .Include(o => o.Orderlines)
                .ThenInclude(ol => ol.Product)
                .Select(o => new OrderDTO
                {

                    Orderdate = o.Orderdate,
                    Statusoforder = o.Statusoforder,
                    Employeeid = o.Employeeid,
                    
                })
                .ToList();

            return Ok(orders);
        }

        [HttpGet("cart")]
        public ActionResult<IEnumerable<ShoppingcartDTO>> GetCart()
        {
            int productid = GetProductId();
            var cartItems = _context.Shoppingcarts
                .Where(ci => ci.Productid == productid)
                .Include(ci => ci.Product)
                .Select(ci => new ShoppingcartDTO
                {
                    Productid = ci.Productid,
                    Quantity = ci.Quantity,
                })
                .ToList();

            return Ok(cartItems);
        }

        [HttpPost("cart/add")]
        public IActionResult AddToCart([FromBody] ShoppingcartDTO cartItemDto)
        {
            int productid = GetProductId();
            var existingItem = _context.Shoppingcarts
                .FirstOrDefault(ci => ci.Productid == cartItemDto.Productid);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDto.Quantity;
            }
            else
            {
                var newItem = new Shoppingcart
                {
                    Productid = cartItemDto.Productid,
                    Quantity = cartItemDto.Quantity
                };
                _context.Shoppingcarts.Add(newItem);
            }

            _context.SaveChanges();
            return Ok("Товар добавлен в корзину");
        }

        [HttpPut("cart/update/{cartItemId}")]
        public IActionResult UpdateCartItem(int cartItemId, [FromBody] ShoppingcartDTO cartItemDto)
        {
            int clientId = GetClientId();
            var cartItem = _context.Shoppingcarts
                .FirstOrDefault(ci => ci.Basketitemid == cartItemId);

            if (cartItem == null)
                return NotFound("Товар в корзине не найден");

            cartItem.Quantity = cartItemDto.Quantity;
            _context.SaveChanges();
            return Ok("Корзина обновлена");
        }

        [HttpDelete("cart/remove/{cartItemId}")]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            int clientId = GetClientId();
            var cartItem = _context.Shoppingcarts
                .FirstOrDefault(ci => ci.Basketitemid == cartItemId);

            if (cartItem == null)
                return NotFound("Товар в корзине не найден");

            _context.Shoppingcarts.Remove(cartItem);
            _context.SaveChanges();
            return Ok("Товар удален из корзины");
        }
    }
}