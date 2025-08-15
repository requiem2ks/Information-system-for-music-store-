using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly PostgresContext db;

        public StorageController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Storage>>> Get()
        {
            return await db.Storages.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Storage>> Get(int id)
        {
            var item = await db.Storages.FirstOrDefaultAsync(x => x.Storageid == id);
            if (item == null)
                return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Storage>> Post([FromBody] StorageDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var item = new Storage
            {
                Productid = dto.Productid,
                Purchaseid = dto.Purchaseid,
                Quantity = dto.Quantity
            };

            db.Storages.Add(item);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = item.Storageid }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Storage>> Put(int id, [FromBody] StorageDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Storages.FirstOrDefaultAsync(x => x.Storageid == id);
            if (existing == null)
                return NotFound();

            existing.Productid = dto.Productid;
            existing.Purchaseid = dto.Purchaseid;
            existing.Quantity = dto.Quantity;

            db.Storages.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Storage>> Delete(int id)
        {
            var item = await db.Storages.FirstOrDefaultAsync(x => x.Storageid == id);
            if (item == null)
                return NotFound();

            db.Storages.Remove(item);
            await db.SaveChangesAsync();
            return Ok(item);
        }
    }
}
