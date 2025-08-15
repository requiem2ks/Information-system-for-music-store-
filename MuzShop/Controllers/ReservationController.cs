using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly PostgresContext db;

        public ReservationController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> Get()
        {
            return await db.Reservations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> Get(int id)
        {
            var reservation = await db.Reservations.FirstOrDefaultAsync(x => x.Reservationid == id);
            if (reservation == null)
                return NotFound();
            return reservation;
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> Post([FromBody] ReservationDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var reservation = new Reservation
            {
                Reservationdate = dto.Reservationdate,
                Reservationenddate = dto.Reservationenddate
            };

            db.Reservations.Add(reservation);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = reservation.Reservationid }, reservation);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Reservation>> Put(int id, [FromBody] ReservationDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Reservations.FirstOrDefaultAsync(x => x.Reservationid == id);
            if (existing == null)
                return NotFound();

            existing.Reservationdate = dto.Reservationdate;
            existing.Reservationenddate = dto.Reservationenddate;

            db.Reservations.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Reservation>> Delete(int id)
        {
            var reservation = await db.Reservations.FirstOrDefaultAsync(x => x.Reservationid == id);
            if (reservation == null)
                return NotFound();

            db.Reservations.Remove(reservation);
            await db.SaveChangesAsync();
            return Ok(reservation);
        }

        
    }
}
