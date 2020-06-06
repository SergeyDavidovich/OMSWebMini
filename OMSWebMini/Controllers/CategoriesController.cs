using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWebService.Data;
using OMSWebService.Model;

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
        public async Task<ActionResult<IEnumerable>> GetCategories(
            bool include_pictures = false)
        {
            if (include_pictures)
            {
                var result = await _context.Categories
                    .Select(
                    c => new
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
                var result = await _context.Categories.
                    Select(
                    c => new
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
        public async Task<ActionResult<object>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();
            else
                return new
                {
                    CategoryID = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description
                };
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromBody] Category item)
        {
            _context.Categories.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories),
                new
                {
                    Id = item.CategoryId,
                    CategoryName = item.CategoryName,
                    Description = item.Description
                },
                item);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
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

            return NoContent();
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