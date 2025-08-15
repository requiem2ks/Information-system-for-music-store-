using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PostgresContext _db;

        public PaymentController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> Get()
        {
            return await _db.Payments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> Get(int id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();
            return payment;
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> Post([FromBody] PaymentDTO paymentDto)
        {
            if (paymentDto == null)
            {
                return BadRequest();
            }

            var payment = new Payment
            {
                Paymentsum = paymentDto.Paymentsum,
                Dateofpayment = paymentDto.Dateofpayment,
                Paymentmethod = paymentDto.Paymentmethod
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = payment.Paymentid }, payment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Payment>> Put(int id, [FromBody] PaymentDTO paymentDto)
        {
            if (paymentDto == null)
            {
                return BadRequest();
            }

            var existingPayment = await _db.Payments.FindAsync(id);
            if (existingPayment == null)
            {
                return NotFound();
            }

            existingPayment.Paymentsum = paymentDto.Paymentsum;
            existingPayment.Dateofpayment = paymentDto.Dateofpayment;
            existingPayment.Paymentmethod = paymentDto.Paymentmethod;

            _db.Payments.Update(existingPayment);
            await _db.SaveChangesAsync();
            return Ok(existingPayment);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Payment>> Delete(int id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _db.Payments.Remove(payment);
            await _db.SaveChangesAsync();
            return Ok(payment);
        }
    }
}