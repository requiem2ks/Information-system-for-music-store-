using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonaldiscountController : ControllerBase
    {
        private readonly PostgresContext _db;

        public PersonaldiscountController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personaldiscount>>> Get()
        {
            return await _db.Personaldiscounts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Personaldiscount>> Get(int id)
        {
            var discount = await _db.Personaldiscounts.FindAsync(id);
            if (discount == null)
                return NotFound();
            return discount;
        }

        [HttpPost]
        public async Task<ActionResult<Personaldiscount>> Post([FromBody] PersonaldiscountDTO discountDto)
        {
            if (discountDto == null)
            {
                return BadRequest("Discount data is required");
            }

            var discount = new Personaldiscount
            {
                Clientid = discountDto.Clientid,
                Discountvalue = discountDto.Discountvalue,
                Startdate = discountDto.Startdate,
                Enddate = discountDto.Enddate
            };

            _db.Personaldiscounts.Add(discount);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = discount.Personaldiscountid }, discount);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Personaldiscount>> Put(int id, [FromBody] PersonaldiscountDTO discountDto)
        {
            if (discountDto == null)
            {
                return BadRequest("Discount data is required");
            }

            var existingDiscount = await _db.Personaldiscounts.FindAsync(id);
            if (existingDiscount == null)
            {
                return NotFound();
            }

            existingDiscount.Clientid = discountDto.Clientid;
            existingDiscount.Discountvalue = discountDto.Discountvalue;
            existingDiscount.Startdate = discountDto.Startdate;
            existingDiscount.Enddate = discountDto.Enddate;

            _db.Personaldiscounts.Update(existingDiscount);
            await _db.SaveChangesAsync();
            return Ok(existingDiscount);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Personaldiscount>> Delete(int id)
        {
            var discount = await _db.Personaldiscounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            _db.Personaldiscounts.Remove(discount);
            await _db.SaveChangesAsync();
            return Ok(discount);
        }
    }
}