using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MuzShop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Требует авторизации
public class ProfileController : ControllerBase
{
    private readonly PostgresContext _context;

    public ProfileController(PostgresContext context)
    {
        _context = context;
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserProfile()
    {
        // Получаем ID пользователя из токена
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var user = await _context.Users
            .Include(u => u.Client)
            .Include(u => u.Employee)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Userid == userId);

        if (user == null)
            return NotFound("Пользователь не найден");

        // Формируем ответ в зависимости от роли
        if (user.Role?.Rolename == "client" && user.Client != null)
        {
            return Ok(new
            {
                user.Email,
                user.Client.Fio,
                user.Client.Phone,
                Role = user.Role?.Rolename
            });
        }
        else if (user.Role?.Rolename == "employee" && user.Employee != null)
        {
            return Ok(new
            {
                user.Email,
                user.Employee.Fio,
                user.Employee.Phone,
                JobTitle = user.Employee.Jobtitle,
                Role = user.Role?.Rolename
            });
        }

        return BadRequest("Не удалось получить данные профиля");
    }
}