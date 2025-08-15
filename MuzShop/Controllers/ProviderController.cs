using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly PostgresContext db;

        public ProviderController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provider>>> Get()
        {
            return await db.Providers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> Get(int id)
        {
            var provider = await db.Providers.FirstOrDefaultAsync(x => x.Providerid == id);
            if (provider == null)
                return NotFound();
            return provider;
        }

        [HttpPost]
        public async Task<ActionResult<Provider>> Post([FromBody] ProviderDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var provider = new Provider
            {
                Nameprovider = dto.Nameprovider,
                Phone = dto.Phone,
                Address = dto.Address
            };

            db.Providers.Add(provider);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = provider.Providerid }, provider);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Provider>> Put(int id, [FromBody] ProviderDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Providers.FirstOrDefaultAsync(x => x.Providerid == id);
            if (existing == null)
                return NotFound();

            existing.Nameprovider = dto.Nameprovider;
            existing.Phone = dto.Phone;
            existing.Address = dto.Address;

            db.Providers.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Provider>> Delete(int id)
        {
            var provider = await db.Providers.FirstOrDefaultAsync(x => x.Providerid == id);
            if (provider == null)
                return NotFound();

            db.Providers.Remove(provider);
            await db.SaveChangesAsync();
            return Ok(provider);
        }
    }
}
