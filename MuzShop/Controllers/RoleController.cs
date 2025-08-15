using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly PostgresContext _db;

        public RoleController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> Get()
        {
            return await _db.Roles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> Get(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null)
                return NotFound();
            return role;
        }

        [HttpPost]
        public async Task<ActionResult<Role>> Post([FromBody] RoleDTO roleDto)
        {
            if (roleDto == null || string.IsNullOrWhiteSpace(roleDto.RoleName))
            {
                return BadRequest("Role name is required");
            }

            var role = new Role
            {
                Rolename = roleDto.RoleName
            };

            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = role.Roleid }, role);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Role>> Put(int id, [FromBody] RoleDTO roleDto)
        {
            if (roleDto == null || string.IsNullOrWhiteSpace(roleDto.RoleName))
            {
                return BadRequest("Role name is required");
            }

            var existingRole = await _db.Roles.FindAsync(id);
            if (existingRole == null)
            {
                return NotFound();
            }

            existingRole.Rolename = roleDto.RoleName;

            _db.Roles.Update(existingRole);
            await _db.SaveChangesAsync();
            return Ok(existingRole);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Role>> Delete(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
            return Ok(role);
        }
    }
}