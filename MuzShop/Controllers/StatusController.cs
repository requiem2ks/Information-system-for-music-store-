using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly PostgresContext db;

        public StatusController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Status>>> Get()
        {
            return await db.Statuses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> Get(int id)
        {
            var item = await db.Statuses.FirstOrDefaultAsync(x => x.Statusid == id);
            if (item == null)
                return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Status>> Post([FromBody] StatusDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var item = new Status
            {
                Statusid = dto.Statusid,
                Namestatus = dto.Namestatus
            };

            db.Statuses.Add(item);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = item.Statusid }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Status>> Put(int id, [FromBody] StatusDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Statuses.FirstOrDefaultAsync(x => x.Statusid == id);
            if (existing == null)
                return NotFound();

            existing.Namestatus = dto.Namestatus;

            db.Statuses.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Status>> Delete(int id)
        {
            var item = await db.Statuses.FirstOrDefaultAsync(x => x.Statusid == id);
            if (item == null)
                return NotFound();

            db.Statuses.Remove(item);
            await db.SaveChangesAsync();
            return Ok(item);
        }
    }
}
