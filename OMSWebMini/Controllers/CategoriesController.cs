using System;
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
        public async Task<ActionResult<IEnumerable<Categories>>> GetCategories(bool include_pictures = false)
        {
            if (include_pictures)
            {
                return await _context.Categories.ToListAsync();
            }
            else
            {
                var categories = _context.Categories.
                    Select(
                    c => new Categories
                    {
                        CategoryName = c.CategoryName,
                        CategoryId = c.CategoryId,
                        Description = c.Description
                    }
                    );
                return await categories.ToListAsync();
            }
        }
        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Categories>> PostTodoItem(Categories category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return null;
            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            //return CreatedAtAction(nameof(Categories), new { id = category.CategoryId }, todoItem);
        }
    }
}