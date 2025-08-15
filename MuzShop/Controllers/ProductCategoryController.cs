using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuzShop.Models;
using MuzShop;

namespace MuzShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductcategoryController : ControllerBase
    {
        private readonly PostgresContext _db;

        public ProductcategoryController(PostgresContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Productcategory>>> Get()
        {
            return await _db.Productcategories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Productcategory>> Get(int id)
        {
            var category = await _db.Productcategories.FindAsync(id);
            if (category == null)
                return NotFound();
            return category;
        }

        [HttpPost]
        public async Task<ActionResult<Productcategory>> Post([FromBody] ProductcategoryDTO categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Namecategory))
            {
                return BadRequest("Category name is required");
            }

            var category = new Productcategory
            {
                Namecategory = categoryDto.Namecategory,
                Descriptioncategory = categoryDto.Descriptioncategory
            };

            _db.Productcategories.Add(category);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = category.Productcategoryid }, category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Productcategory>> Put(int id, [FromBody] ProductcategoryDTO categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Namecategory))
            {
                return BadRequest("Category name is required");
            }

            var existingCategory = await _db.Productcategories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Namecategory = categoryDto.Namecategory;
            existingCategory.Descriptioncategory = categoryDto.Descriptioncategory;

            _db.Productcategories.Update(existingCategory);
            await _db.SaveChangesAsync();
            return Ok(existingCategory);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Productcategory>> Delete(int id)
        {
            var category = await _db.Productcategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _db.Productcategories.Remove(category);
            await _db.SaveChangesAsync();
            return Ok(category);
        }
    }
}