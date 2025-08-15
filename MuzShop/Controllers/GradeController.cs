using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradeController : ControllerBase
    {
        private readonly PostgresContext _db;

        public GradeController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grade>>> Get()
        {
            return await _db.Grades.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> Get(int id)
        {
            var grade = await _db.Grades.FindAsync(id);
            if (grade == null)
                return NotFound();
            return grade;
        }

        [HttpPost]
        public async Task<ActionResult<Grade>> Post([FromBody] GradeDTO gradeDto)
        {
            if (gradeDto == null)
            {
                return BadRequest();
            }

            var grade = new Grade
            {
                Valuegrade = gradeDto.Valuegrade
            };

            _db.Grades.Add(grade);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = grade.Gradeid }, grade);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Grade>> Put(int id, [FromBody] GradeDTO gradeDto)
        {
            if (gradeDto == null)
            {
                return BadRequest();
            }

            var existingGrade = await _db.Grades.FindAsync(id);
            if (existingGrade == null)
            {
                return NotFound();
            }

            existingGrade.Valuegrade = gradeDto.Valuegrade;

            _db.Grades.Update(existingGrade);
            await _db.SaveChangesAsync();
            return Ok(existingGrade);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Grade>> Delete(int id)
        {
            var grade = await _db.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            _db.Grades.Remove(grade);
            await _db.SaveChangesAsync();
            return Ok(grade);
        }
    }
}