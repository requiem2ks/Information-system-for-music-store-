using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly PostgresContext db;

        public ClientController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> Get()
        {
            return await db.Clients.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> Get(int id)
        {
            var attendance = await db.Clients.FirstOrDefaultAsync(x => x.Clientid == id);
            if (attendance == null)
                return NotFound();
            return attendance;
        }

        [HttpPost]
        public async Task<ActionResult<Client>> Post([FromBody] ClientDTO clientDto)
        {
            if (clientDto == null)
            {
                return BadRequest();
            }

            var client = new Client
            {
                Fio = clientDto.Fio,
                Phone = clientDto.Phone,
                Address = clientDto.Address
            };

            db.Clients.Add(client);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = client.Clientid }, client);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Client>> Put(int id, [FromBody] ClientDTO clientDto)
        {
            if (clientDto == null)
            {
                return BadRequest();
            }

            var existingClient = await db.Clients.FirstOrDefaultAsync(x => x.Clientid == id);
            if (existingClient == null)
            {
                return NotFound();
            }

            existingClient.Fio = clientDto.Fio;
            existingClient.Phone = clientDto.Phone;
            existingClient.Address = clientDto.Address;

            db.Clients.Update(existingClient);
            await db.SaveChangesAsync();
            return Ok(existingClient);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var client = await db.Clients
                .Include(c => c.Orders)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(x => x.Clientid == id);

            if (client == null) return NotFound();

            if (client.Orders.Any())
            {
                return BadRequest("Нельзя удалить клиента, у которого есть заказы");
            }

            if (client.Users.Any())
            {
                return BadRequest("Нельзя удалить клиента, у которого есть учетная запись");
            }

            db.Clients.Remove(client);
            await db.SaveChangesAsync();
            return Ok(client);
        }
    }
}