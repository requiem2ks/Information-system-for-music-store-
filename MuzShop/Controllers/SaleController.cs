using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly PostgresContext db;

        public SaleController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> Get()
        {
            return await db.Sales.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> Get(int id)
        {
            var sale = await db.Sales.FirstOrDefaultAsync(x => x.Saleid == id);
            if (sale == null)
                return NotFound();
            return sale;
        }

        [HttpPost]
        public async Task<ActionResult<Sale>> Post([FromBody] SaleDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var sale = new Sale
            {
                Orderid = dto.Orderid,
                Saledate = dto.Saledate,
                Totalamount = dto.Totalamount,
                Profitamount = dto.Profitamount
            };

            db.Sales.Add(sale);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = sale.Saleid }, sale);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Sale>> Put(int id, [FromBody] SaleDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Sales.FirstOrDefaultAsync(x => x.Saleid == id);
            if (existing == null)
                return NotFound();

            existing.Orderid = dto.Orderid;
            existing.Saledate = dto.Saledate;
            existing.Totalamount = dto.Totalamount;
            existing.Profitamount = dto.Profitamount;

            db.Sales.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Sale>> Delete(int id)
        {
            var sale = await db.Sales.FirstOrDefaultAsync(x => x.Saleid == id);
            if (sale == null)
                return NotFound();

            db.Sales.Remove(sale);
            await db.SaveChangesAsync();
            return Ok(sale);
        }
    }
}
