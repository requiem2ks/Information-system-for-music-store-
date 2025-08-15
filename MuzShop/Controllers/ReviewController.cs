using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly PostgresContext db;

        public ReviewController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> Get()
        {
            return await db.Reviews.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> Get(int id)
        {
            var review = await db.Reviews.FirstOrDefaultAsync(x => x.Reviewid == id);
            if (review == null)
                return NotFound();
            return review;
        }

        [HttpPost]
        public async Task<ActionResult<Review>> Post([FromBody] ReviewDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var review = new Review
            {
                Clientid = dto.Clientid,
                Productid = dto.Productid,
                Rewievtext = dto.Rewievtext,
                Grade = dto.Grade,
                Dateofrevocation = dto.Dateofrevocation
            };

            db.Reviews.Add(review);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = review.Reviewid }, review);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Review>> Put(int id, [FromBody] ReviewDTO dto)
        {
            if (dto == null)
                return BadRequest();

            var existing = await db.Reviews.FirstOrDefaultAsync(x => x.Reviewid == id);
            if (existing == null)
                return NotFound();

            existing.Clientid = dto.Clientid;
            existing.Productid = dto.Productid;
            existing.Rewievtext = dto.Rewievtext;
            existing.Grade = dto.Grade;
            existing.Dateofrevocation = dto.Dateofrevocation;

            db.Reviews.Update(existing);
            await db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Review>> Delete(int id)
        {
            var review = await db.Reviews.FirstOrDefaultAsync(x => x.Reviewid == id);
            if (review == null)
                return NotFound();

            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
            return Ok(review);
        }
    }
}
