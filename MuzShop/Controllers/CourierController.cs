using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourierController : ControllerBase
    {
        private readonly PostgresContext db;

        public CourierController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Courier>>> Get()
        {
            return await db.Couriers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Courier>> Get(int id)
        {
            var courier = await db.Couriers.FirstOrDefaultAsync(x => x.Courierid == id);
            if (courier == null)
                return NotFound();
            return courier;
        }

        [HttpPost]
        public async Task<ActionResult<Courier>> Post([FromBody] CourierDTO courierDto)
        {
            if (courierDto == null)
            {
                return BadRequest();
            }

            var courier = new Courier
            {
                Fio = courierDto.Fio,
                Typeoftransport = courierDto.Typeoftransport,
                Phone = courierDto.Phone,
                Email = courierDto.Email,
                Liftingcapacity = courierDto.Liftingcapacity,
                Maximumsize = courierDto.Maximumsize
            };

            db.Couriers.Add(courier);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = courier.Courierid }, courier);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Courier>> Put(int id, [FromBody] CourierDTO courierDTO)
        {
            if (courierDTO == null)
            {
                return BadRequest();
            }

            var existingCourier = await db.Couriers.FirstOrDefaultAsync(x => x.Courierid == id);
            if (existingCourier == null)
            {
                return NotFound();
            }

            existingCourier.Fio = courierDTO.Fio;
            existingCourier.Typeoftransport = courierDTO.Typeoftransport;
            existingCourier.Phone = courierDTO.Phone;
            existingCourier.Email = courierDTO.Email;
            existingCourier.Liftingcapacity = courierDTO.Liftingcapacity;
            existingCourier.Maximumsize = courierDTO.Maximumsize;

            db.Couriers.Update(existingCourier);
            await db.SaveChangesAsync();
            return Ok(existingCourier);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Courier>> Delete(int id)
        {
            var courier = await db.Couriers.FirstOrDefaultAsync(x => x.Courierid == id);
            if (courier == null)
            {
                return NotFound();
            }

            db.Couriers.Remove(courier);
            await db.SaveChangesAsync();
            return Ok(courier);
        }
        ////
    }
}