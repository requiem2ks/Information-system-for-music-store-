using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly PostgresContext db;

        public PromotionController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> Get()
        {
            return await db.Promotions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Promotion>> Get(int id)
        {
            var promo = await db.Promotions.FirstOrDefaultAsync(x => x.Stockid == id);
            if (promo == null)
                return NotFound();
            return promo;
        }

        [HttpPost]
        public async Task<ActionResult<Promotion>> Post([FromBody] PromotionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var promo = new Promotion
            {
                Namestock = dto.Namestock,
                Description = dto.Description,
                Startdate = dto.Startdate,
                Enddate = dto.Enddate,
                Typeofstock = dto.Typeofstock,
                Discountvalue = dto.Discountvalue
            };

            db.Promotions.Add(promo);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = promo.Stockid }, promo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Promotion>> Put(int id, [FromBody] PromotionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var existing = await db.Promotions.FirstOrDefaultAsync(x => x.Stockid == id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Namestock = dto.Namestock;
            existing.Description = dto.Description;
            existing.Startdate = dto.Startdate;
            existing.Enddate = dto.Enddate;
            existing.Typeofstock = dto.Typeofstock;
            existing.Discountvalue = dto.Discountvalue;

            db.Promotions.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Promotion>> Delete(int id)
        {
            var promo = await db.Promotions.FirstOrDefaultAsync(x => x.Stockid == id);
            if (promo == null)
            {
                return NotFound();
            }

            db.Promotions.Remove(promo);
            await db.SaveChangesAsync();
            return Ok(promo);
        }
    }
}
