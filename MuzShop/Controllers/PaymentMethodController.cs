using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentmethodController : ControllerBase
    {
        private readonly PostgresContext _db;

        public PaymentmethodController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paymentmethod>>> Get()
        {
            return await _db.Paymentmethods.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Paymentmethod>> Get(int id)
        {
            var paymentMethod = await _db.Paymentmethods.FindAsync(id);
            if (paymentMethod == null)
                return NotFound();
            return paymentMethod;
        }

        [HttpPost]
        public async Task<ActionResult<Paymentmethod>> Post([FromBody] PaymentmethodDTO paymentMethodDto)
        {
            if (paymentMethodDto == null || string.IsNullOrEmpty(paymentMethodDto.Paymentmethodname))
            {
                return BadRequest("Payment method name is required");
            }

            var paymentMethod = new Paymentmethod
            {
                Paymentmethodname = paymentMethodDto.Paymentmethodname
            };

            _db.Paymentmethods.Add(paymentMethod);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = paymentMethod.Paymentmethodid }, paymentMethod);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Paymentmethod>> Put(int id, [FromBody] PaymentmethodDTO paymentMethodDto)
        {
            if (paymentMethodDto == null || string.IsNullOrEmpty(paymentMethodDto.Paymentmethodname))
            {
                return BadRequest("Payment method name is required");
            }

            var existingPaymentMethod = await _db.Paymentmethods.FindAsync(id);
            if (existingPaymentMethod == null)
            {
                return NotFound();
            }

            existingPaymentMethod.Paymentmethodname = paymentMethodDto.Paymentmethodname;

            _db.Paymentmethods.Update(existingPaymentMethod);
            await _db.SaveChangesAsync();
            return Ok(existingPaymentMethod);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Paymentmethod>> Delete(int id)
        {
            var paymentMethod = await _db.Paymentmethods.FindAsync(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            _db.Paymentmethods.Remove(paymentMethod);
            await _db.SaveChangesAsync();
            return Ok(paymentMethod);
        }
    }
}