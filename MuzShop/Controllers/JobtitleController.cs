using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobtitleController : ControllerBase
    {
        private readonly PostgresContext _db;

        public JobtitleController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jobtitle>>> Get()
        {
            return await _db.Jobtitles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Jobtitle>> Get(int id)
        {
            var jobtitle = await _db.Jobtitles.FindAsync(id);
            if (jobtitle == null)
                return NotFound();
            return jobtitle;
        }

        [HttpPost]
        public async Task<ActionResult<Jobtitle>> Post([FromBody] JobtitleDTO jobtitleDto)
        {
            if (jobtitleDto == null)
            {
                return BadRequest();
            }

            var jobtitle = new Jobtitle
            {
                Name = jobtitleDto.Name
            };

            _db.Jobtitles.Add(jobtitle);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = jobtitle.Jobtitleid }, jobtitle);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Jobtitle>> Put(int id, [FromBody] JobtitleDTO jobtitleDto)
        {
            if (jobtitleDto == null)
            {
                return BadRequest();
            }

            var existingJobtitle = await _db.Jobtitles.FindAsync(id);
            if (existingJobtitle == null)
            {
                return NotFound();
            }

            existingJobtitle.Name = jobtitleDto.Name;

            _db.Jobtitles.Update(existingJobtitle);
            await _db.SaveChangesAsync();
            return Ok(existingJobtitle);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Jobtitle>> Delete(int id)
        {
            var jobtitle = await _db.Jobtitles.FindAsync(id);
            if (jobtitle == null)
            {
                return NotFound();
            }

            _db.Jobtitles.Remove(jobtitle);
            await _db.SaveChangesAsync();
            return Ok(jobtitle);
        }
    }
}