using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionProductController : ControllerBase
    {
        private readonly PostgresContext db;

        public PromotionProductController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotionproduct>>> Get()
        {
            return await db.Promotionproducts.ToListAsync();
        }

        [HttpGet("{stockid}/{productid}")]
        public async Task<ActionResult<Promotionproduct>> Get(int stockid, int productid)
        {
            var entry = await db.Promotionproducts
                .FirstOrDefaultAsync(x => x.Stockid == stockid && x.Productid == productid);

            if (entry == null)
                return NotFound();

            return entry;
        }

        [HttpPost]
        public async Task<ActionResult<Promotionproduct>> Post([FromBody] PromotionproductDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var entry = new Promotionproduct
            {
                Stockid = dto.Stockid,
                Productid = dto.Productid
            };

            db.Promotionproducts.Add(entry);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { stockid = entry.Stockid, productid = entry.Productid }, entry);
        }

        [HttpDelete("{stockid}/{productid}")]
        public async Task<ActionResult<Promotionproduct>> Delete(int stockid, int productid)
        {
            var entry = await db.Promotionproducts
                .FirstOrDefaultAsync(x => x.Stockid == stockid && x.Productid == productid);

            if (entry == null)
                return NotFound();

            db.Promotionproducts.Remove(entry);
            await db.SaveChangesAsync();
            return Ok(entry);
        }
    }
}
