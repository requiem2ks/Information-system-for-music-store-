using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;


namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly PostgresContext _db;

        public UserController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await _db.Users
                .Include(u => u.Role) // Опционально: подгружаем связанную роль
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _db.Users
                .Include(u => u.Role) // Опционально: подгружаем связанную роль
                .FirstOrDefaultAsync(x => x.Userid == id);
            
            if (user == null)
                return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] UserDTO userDto)
        {
            if (userDto == null || string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Hashpassword))
            {
                return BadRequest("Email and password are required");
            }

            // Проверка существования роли (если указана)
            if (userDto.Roleid.HasValue && !await _db.Roles.AnyAsync(r => r.Roleid == userDto.Roleid))
            {
                return BadRequest("Specified role does not exist");
            }

            var user = new User
            {
                Roleid = userDto.Roleid,
                Clientid = userDto.Clientid,
                Employeeid = userDto.Employeeid,
                Email = userDto.Email,
                Hashpassword = userDto.Hashpassword // На практике пароль должен хешироваться здесь
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Userid }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put(int id, [FromBody] UserDTO userDto)
        {
            if (userDto == null || string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Hashpassword))
            {
                return BadRequest("Email and password are required");
            }

            // Проверка существования роли (если указана)
            if (userDto.Roleid.HasValue && !await _db.Roles.AnyAsync(r => r.Roleid == userDto.Roleid))
            {
                return BadRequest("Specified role does not exist");
            }

            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Userid == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Roleid = userDto.Roleid;
            existingUser.Clientid = userDto.Clientid;
            existingUser.Employeeid = userDto.Employeeid;
            existingUser.Email = userDto.Email;
            existingUser.Hashpassword = userDto.Hashpassword; // На практике пароль должен хешироваться здесь

            _db.Users.Update(existingUser);
            await _db.SaveChangesAsync();
            return Ok(existingUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(user);
        }
    }
}