using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly PostgresContext db;

        public DeliveryController(PostgresContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Delivery>>> Get()
        {
            return await db.Deliveries.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Delivery>> Get(int id)
        {
            var delivery = await db.Deliveries.FirstOrDefaultAsync(x => x.Deliveryid == id);
            if (delivery == null)
                return NotFound();
            return delivery;
        }

        [HttpPost]
        public async Task<ActionResult<Delivery>> Post([FromBody] DeliveryDTO deliveryDto)
        {
            if (deliveryDto == null)
            {
                return BadRequest();
            }

            var delivery = new Delivery
            {
                Orderid = deliveryDto.Orderid,
                Courierid = deliveryDto.Courierid,
                Deliveryaddress = deliveryDto.Deliveryaddress,
                Statusofdelivery = deliveryDto.Statusofdelivery,
                Dateofdelivery = deliveryDto.Dateofdelivery
            };

            db.Deliveries.Add(delivery);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = delivery.Deliveryid }, delivery);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Delivery>> Put(int id, [FromBody] DeliveryDTO deliveryDTO)
        {
            if (deliveryDTO == null)
            {
                return BadRequest();
            }

            var existingDelivery = await db.Deliveries.FirstOrDefaultAsync(x => x.Deliveryid == id);
            if (existingDelivery == null)
            {
                return NotFound();
            }

            existingDelivery.Courierid = deliveryDTO.Courierid;
            existingDelivery.Deliveryaddress = deliveryDTO.Deliveryaddress;
            existingDelivery.Statusofdelivery = deliveryDTO.Statusofdelivery;
            existingDelivery.Dateofdelivery = deliveryDTO.Dateofdelivery;

            db.Deliveries.Update(existingDelivery);
            await db.SaveChangesAsync();
            return Ok(existingDelivery);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Delivery>> Delete(int id)
        {
            var delivery = await db.Deliveries.FirstOrDefaultAsync(x => x.Deliveryid == id);
            if (delivery == null)
            {
                return NotFound();
            }

            db.Deliveries.Remove(delivery);
            await db.SaveChangesAsync();
            return Ok(delivery);
        }
    }
}