using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using OMSWebService.Model;
using OMSWebService.Data;
using System.Collections;

namespace OMSWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly NORTHWNDContext _context;

        public ProductsController(NORTHWNDContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Supplier = p.Supplier,
                    Category = p.Category,
                    UnitPrice = p.UnitPrice,
                    UnitsInStock = p.UnitsInStock,
                    Discontinued = p.Discontinued
                }).ToListAsync();
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product item)
        {
            //Product product = new Product()
            //{
            //    ProductName = item.ProductName
            //};

            _context.Products.Add(item);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducts),
                new
                {
                    ProductId = item.ProductId
                },
                item);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product item)
        {
            if (id != item.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var todoItem = await _context.Products.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.Products.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
