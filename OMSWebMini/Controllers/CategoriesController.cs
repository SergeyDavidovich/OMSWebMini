using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWebMini.Data;
using OMSWebMini.Model;
namespace OMSWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly NORTHWNDContext _context;

        public CategoriesController(NORTHWNDContext context)
        {
            _context = context;
        }

        // GET: api/categories?include_picture=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(
            bool include_pictures = false)
        {
            if (include_pictures)
            {
                var result = await _context.Categories
                    .Select(
                    c => new Category
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        Picture = c.Picture
                    }).ToListAsync();

                return result;
            }
            else
            {
                var result = await  _context.Categories.
                    Select(
                    c => new Category
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        Description = c.Description
                    }).ToListAsync();

                return result;
            }
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();
            else
                return new Category
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                };
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromBody] Category item)
        {
            Category category = new Category()
            {
                CategoryName = item.CategoryName,
                Description = item.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var xxx = nameof(GetCategory);

            return CreatedAtAction(nameof(GetCategory),
                new
                {
                    Id = item.CategoryId
                },
                category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody]Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return category;
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}