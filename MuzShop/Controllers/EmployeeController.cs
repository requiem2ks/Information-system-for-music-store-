using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly PostgresContext _db;

        public EmployeeController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Get()
        {
            return await _db.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();
            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Post([FromBody] EmployeeDTO employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest();
            }

            var employee = new Employee
            {
                Jobtitle = employeeDto.Jobtitle,
                Fio = employeeDto.Fio,
                Phone = employeeDto.Phone,
                Email = employeeDto.Email
            };

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = employee.Employeeid }, employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> Put(int id, [FromBody] EmployeeDTO employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest();
            }

            var existingEmployee = await _db.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.Jobtitle = employeeDto.Jobtitle;
            existingEmployee.Fio = employeeDto.Fio;
            existingEmployee.Phone = employeeDto.Phone;
            existingEmployee.Email = employeeDto.Email;

            _db.Employees.Update(existingEmployee);
            await _db.SaveChangesAsync();
            return Ok(existingEmployee);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> Delete(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            return Ok(employee);
        }
    }
}