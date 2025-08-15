using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly PostgresContext db;

        public PurchaseController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> Get()
        {
            return await db.Purchases.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> Get(int id)
        {
            var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Purchaseid == id);
            if (purchase == null)
                return NotFound();
            return purchase;
        }

        [HttpPost]
        public async Task<ActionResult<Purchase>> Post([FromBody] PurchaseDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var purchase = new Purchase
            {
                Providerid = dto.Providerid,
                Productid = dto.Productid,
                Unitprice = dto.Unitprice,
                Quantity = dto.Quantity,
                Datepurchase = dto.Datepurchase
            };

            db.Purchases.Add(purchase);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = purchase.Purchaseid }, purchase);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Purchase>> Put(int id, [FromBody] PurchaseDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Purchases.FirstOrDefaultAsync(x => x.Purchaseid == id);
            if (existing == null)
                return NotFound();

            existing.Providerid = dto.Providerid;
            existing.Productid = dto.Productid;
            existing.Unitprice = dto.Unitprice;
            existing.Quantity = dto.Quantity;
            existing.Datepurchase = dto.Datepurchase;

            db.Purchases.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Purchase>> Delete(int id)
        {
            var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Purchaseid == id);
            if (purchase == null)
                return NotFound();

            db.Purchases.Remove(purchase);
            await db.SaveChangesAsync();
            return Ok(purchase);
        }
    }
}
