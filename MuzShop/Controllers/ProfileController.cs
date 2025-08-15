using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MuzShop.Models;
using BCrypt.Net;

using MuzShop;

namespace MuzShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly PostgresContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(PostgresContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    private async Task<int> GetRoleId(string roleName)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Rolename == roleName);
        if (role == null)
            throw new Exception($"Роль '{roleName}' не найдена");
        return role.Roleid;
    }

    [HttpPost("register/client")]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterClientDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Пользователь с таким Email уже существует");

        // Ищем  по ФИО
        var client = await _context.Clients.FirstOrDefaultAsync(p => p.Fio == dto.Fio);
        if (client == null)
        {
            // Если нет — создаем нового 
            client = new Client
            {
                Fio = dto.Fio,
                Phone = dto.phone,
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Email = dto.Email,
            Hashpassword = hash,
            Roleid = await GetRoleId("client"),
            Clientid = client.Clientid
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Клиент успешно зарегистрирован");
    }

    [HttpPost("register/employee")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Пользователь с таким Email уже существует");

        // Ищем сотрудника по ФИО
        var employee = await _context.Employees.FirstOrDefaultAsync(w => w.Fio == dto.Fio);
        if (employee == null)
        {
            // Если нет — создаем нового сотрудника
            employee = new Employee
            {
                Fio = dto.Fio,
                Phone = dto.phone,
                Jobtitle = dto.jobtitle,
                Email = dto.Email
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Email = dto.Email,
            Hashpassword = hash,
            Roleid = await GetRoleId("employee"),
            Employeeid = employee.Employeeid
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Сотрудник успешно зарегистрирован");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Hashpassword))
            return Unauthorized("Неверный логин или пароль");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Userid.ToString()),
            new Claim(ClaimTypes.Role, user.Role?.Rolename ?? "User")
        };

        if (user.Clientid.HasValue)
            claims.Add(new Claim("Clientid", user.Clientid.Value.ToString()));

        if (user.Employeeid.HasValue)
            claims.Add(new Claim("Employeeid", user.Employeeid.Value.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
    [HttpGet("me")]
    [Authorize] // Требует валидный JWT
    public async Task<IActionResult> GetCurrentUser()
    {
        // Получаем ID пользователя из токена
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        // Ищем пользователя в БД
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Client)
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Userid == int.Parse(userId));

        if (user == null)
            return NotFound("Пользователь не найден");

        // Формируем ответ
        return Ok(new
        {
            user.Email,
            user.Role?.Rolename,
            ClientId = user.Clientid,
            EmployeeId = user.Employeeid,
            // Добавьте другие поля, если нужно
        });
    }
}
