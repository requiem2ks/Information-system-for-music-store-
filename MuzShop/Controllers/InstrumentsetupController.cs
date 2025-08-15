using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;
using Microsoft.AspNetCore.Authorization;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstrumentsetupController : ControllerBase
    {
        private readonly PostgresContext _db;

        public InstrumentsetupController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        [Authorize(Roles = "client, admin,worker")]
        public async Task<ActionResult<IEnumerable<Instrumentsetup>>> Get()
        {
            return await _db.Instrumentsetups.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "client, admin,worker")]
        public async Task<ActionResult<Instrumentsetup>> Get(int id)
        {
            var setup = await _db.Instrumentsetups.FindAsync(id);
            if (setup == null)
                return NotFound();
            return setup;
        }

        [HttpPost]
        [Authorize(Roles = "admin,worker")]
        public async Task<ActionResult<Instrumentsetup>> Post([FromBody] InstrumentsetupDTO setupDto)
        {
            if (setupDto == null)
            {
                return BadRequest();
            }

            var setup = new Instrumentsetup
            {
                Productid = setupDto.Productid,
                Settingtype = setupDto.Settingtype,
                Descriptionsetting = setupDto.Descriptionsetting,
                Setupcost = setupDto.Setupcost
            };

            _db.Instrumentsetups.Add(setup);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = setup.Settingid }, setup);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,worker")]
        public async Task<ActionResult<Instrumentsetup>> Put(int id, [FromBody] InstrumentsetupDTO setupDto)
        {
            if (setupDto == null)
            {
                return BadRequest();
            }

            var existingSetup = await _db.Instrumentsetups.FindAsync(id);
            if (existingSetup == null)
            {
                return NotFound();
            }

            existingSetup.Productid = setupDto.Productid;
            existingSetup.Settingtype = setupDto.Settingtype;
            existingSetup.Descriptionsetting = setupDto.Descriptionsetting;
            existingSetup.Setupcost = setupDto.Setupcost;

            _db.Instrumentsetups.Update(existingSetup);
            await _db.SaveChangesAsync();
            return Ok(existingSetup);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,worker")]
        public async Task<ActionResult<Instrumentsetup>> Delete(int id)
        {
            var setup = await _db.Instrumentsetups.FindAsync(id);
            if (setup == null)
            {
                return NotFound();
            }

            _db.Instrumentsetups.Remove(setup);
            await _db.SaveChangesAsync();
            return Ok(setup);
        }
    }
}