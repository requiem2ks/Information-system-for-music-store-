using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptsatthestorageController : ControllerBase
    {
        private readonly PostgresContext db;

        public ReceiptsatthestorageController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receiptsatthestorage>>> Get()
        {
            return await db.Receiptsatthestorages.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Receiptsatthestorage>> Get(int id)
        {
            var receipt = await db.Receiptsatthestorages.FirstOrDefaultAsync(x => x.Receiptsatthestoragesid == id);
            if (receipt == null)
                return NotFound();
            return receipt;
        }

        [HttpPost]
        public async Task<ActionResult<Receiptsatthestorage>> Post([FromBody] ReceiptsatthestorageDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var receipt = new Receiptsatthestorage
            {
                Purchaseid = dto.Purchaseid,
                Storageid = dto.Storageid,
                Dateofreceipt = dto.Dateofreceipt
            };

            db.Receiptsatthestorages.Add(receipt);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = receipt.Receiptsatthestoragesid }, receipt);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Receiptsatthestorage>> Put(int id, [FromBody] ReceiptsatthestorageDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Receiptsatthestorages.FirstOrDefaultAsync(x => x.Receiptsatthestoragesid == id);
            if (existing == null)
                return NotFound();

            existing.Purchaseid = dto.Purchaseid;
            existing.Storageid = dto.Storageid;
            existing.Dateofreceipt = dto.Dateofreceipt;

            db.Receiptsatthestorages.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Receiptsatthestorage>> Delete(int id)
        {
            var receipt = await db.Receiptsatthestorages.FirstOrDefaultAsync(x => x.Receiptsatthestoragesid == id);
            if (receipt == null)
                return NotFound();

            db.Receiptsatthestorages.Remove(receipt);
            await db.SaveChangesAsync();
            return Ok(receipt);
        }
    }
}
