using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductReservationController : ControllerBase
    {
        private readonly PostgresContext db;

        public ProductReservationController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Productreservation>>> Get()
        {
            return await db.Productreservations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Productreservation>> Get(int id)
        {
            var reservation = await db.Productreservations.FirstOrDefaultAsync(x => x.Reservationid == id);
            if (reservation == null)
                return NotFound();
            return reservation;
        }

        [HttpPost]
        public async Task<ActionResult<Productreservation>> Post([FromBody] ProductreservationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var reservation = new Productreservation
            {
                Reservationid = dto.Reservationid,
                Orderid = dto.Orderid,
                Productid = dto.Productid,
                Quantityofreserved = dto.Quantityofreserved,
                Unitpricereservation = dto.Unitpricereservation,
                Prepaymentpercentage = dto.Prepaymentpercentage
            };

            db.Productreservations.Add(reservation);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = reservation.Reservationid }, reservation);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Productreservation>> Put(int id, [FromBody] ProductreservationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var existing = await db.Productreservations.FirstOrDefaultAsync(x => x.Reservationid == id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Orderid = dto.Orderid;
            existing.Productid = dto.Productid;
            existing.Quantityofreserved = dto.Quantityofreserved;
            existing.Unitpricereservation = dto.Unitpricereservation;
            existing.Prepaymentpercentage = dto.Prepaymentpercentage;

            db.Productreservations.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Productreservation>> Delete(int id)
        {
            var reservation = await db.Productreservations.FirstOrDefaultAsync(x => x.Reservationid == id);
            if (reservation == null)
            {
                return NotFound();
            }

            db.Productreservations.Remove(reservation);
            await db.SaveChangesAsync();
            return Ok(reservation);
        }
    }
}
