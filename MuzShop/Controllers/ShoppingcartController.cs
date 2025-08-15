using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingcartController : ControllerBase
    {
        private readonly PostgresContext db;

        public ShoppingcartController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shoppingcart>>> Get()
        {
            return await db.Shoppingcarts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shoppingcart>> Get(int id)
        {
            var item = await db.Shoppingcarts.FirstOrDefaultAsync(x => x.Basketitemid == id);
            if (item == null)
                return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Shoppingcart>> Post([FromBody] ShoppingcartDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var item = new Shoppingcart
            {
                Productid = dto.Productid,
                Quantity = dto.Quantity
            };

            db.Shoppingcarts.Add(item);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = item.Basketitemid }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Shoppingcart>> Put(int id, [FromBody] ShoppingcartDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Shoppingcarts.FirstOrDefaultAsync(x => x.Basketitemid == id);
            if (existing == null)
                return NotFound();

            existing.Productid = dto.Productid;
            existing.Quantity = dto.Quantity;

            db.Shoppingcarts.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Shoppingcart>> Delete(int id)
        {
            var item = await db.Shoppingcarts.FirstOrDefaultAsync(x => x.Basketitemid == id);
            if (item == null)
                return NotFound();

            db.Shoppingcarts.Remove(item);
            await db.SaveChangesAsync();
            return Ok(item);
        }
    }
}
