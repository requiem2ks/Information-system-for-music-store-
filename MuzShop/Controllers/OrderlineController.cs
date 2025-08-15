using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderlineController : ControllerBase
    {
        private readonly PostgresContext _db;

        public OrderlineController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orderline>>> Get()
        {
            return await _db.Orderlines.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Orderline>> Get(int id)
        {
            var orderline = await _db.Orderlines.FindAsync(id);
            if (orderline == null)
                return NotFound();
            return orderline;
        }

        [HttpPost]
        public async Task<ActionResult<Orderline>> Post([FromBody] OrderlineDTO orderlineDto)
        {
            if (orderlineDto == null)
            {
                return BadRequest();
            }

            var orderline = new Orderline
            {
                Orderid = orderlineDto.Orderid,
                Productid = orderlineDto.Productid,
                Settingid = orderlineDto.Settingid,
                Discount = orderlineDto.Discount,
                Quantity = orderlineDto.Quantity,
                Unitprice = orderlineDto.Unitprice
            };

            _db.Orderlines.Add(orderline);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = orderline.Orderlineid }, orderline);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Orderline>> Put(int id, [FromBody] OrderlineDTO orderlineDto)
        {
            if (orderlineDto == null)
            {
                return BadRequest();
            }

            var existingOrderline = await _db.Orderlines.FindAsync(id);
            if (existingOrderline == null)
            {
                return NotFound();
            }

            existingOrderline.Orderid = orderlineDto.Orderid;
            existingOrderline.Productid = orderlineDto.Productid;
            existingOrderline.Settingid = orderlineDto.Settingid;
            existingOrderline.Discount = orderlineDto.Discount;
            existingOrderline.Quantity = orderlineDto.Quantity;
            existingOrderline.Unitprice = orderlineDto.Unitprice;

            _db.Orderlines.Update(existingOrderline);
            await _db.SaveChangesAsync();
            return Ok(existingOrderline);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Orderline>> Delete(int id)
        {
            var orderline = await _db.Orderlines.FindAsync(id);
            if (orderline == null)
            {
                return NotFound();
            }

            _db.Orderlines.Remove(orderline);
            await _db.SaveChangesAsync();
            return Ok(orderline);
        }
    }
}