using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using OMSWebMini.Model;
using OMSWebMini.Data;
using System.Collections;
using NSwag.Generation.Processors;

namespace OMSWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly NORTHWNDContext _context;

        public OrdersController(NORTHWNDContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders.Select(o => new Order
            {
                OrderId = o.OrderId,
                CustomerId = o.Customer.CustomerId,
                OrderDate = o.OrderDate,
                ShipRegion = o.ShipRegion,
                ShipPostalCode = o.ShipPostalCode,
                ShipCity = o.ShipCity,
                ShipAddress = o.ShipAddress,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate
            }).ToListAsync();

            return orders;
        }

        //https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Where(o => o.OrderId == id)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            var orderDetails = order.OrderDetails
                .Select(o => new OrderDetails
                {
                    OrderId = o.OrderId,
                    UnitPrice = o.UnitPrice,
                    Quantity = o.Quantity,
                    Discount = o.Discount,
                });

            order.OrderDetails = orderDetails.ToList();
            return order;
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostProduct([FromBody] Order order)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            var result = CreatedAtAction(
                nameof(GetOrder),
                new { Id = order.OrderId },
                order);
            return result;
        }
        // PUT: api/orders/10228
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order item)
        {
            if (id != item.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/orders/10248
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var details = _context.OrderDetails.Where(o => order.OrderId == id);
                    _context.OrderDetails.RemoveRange(details);

                    _context.Remove(order);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        //TODO: Not tested

        // DELETE: api/orders/10248
        [HttpDelete("{id[]}")]
        public async Task<IActionResult> DeleteOrdersRange([FromBody] int[] range)
        {
            List<Order> orders = new List<Order>();
            List<OrderDetails> details = new List<OrderDetails>();

            foreach (int id in range)
            {
                orders.Add(await _context.Orders.FindAsync(id));
            }

            foreach (var item in orders)
            {
                if (orders == null)
                {
                    return NotFound();
                }

                details.Add((_context.OrderDetails.Where(o => o.OrderId == item.OrderId) as OrderDetails));
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.OrderDetails.RemoveRange(details);
                    _context.Orders.RemoveRange(orders);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        // DELETE: api/orders/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteOrder(int id)
        //{
        //    var item = await _context.Orders.FindAsync(id);

        //    if (item == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Orders.Remove(item);

        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }




