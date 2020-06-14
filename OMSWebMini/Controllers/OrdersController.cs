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
        public async Task<ActionResult<Order>> PostProduct(Order order)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrders),
                new Order
                {
                    OrderId = order.OrderId,
                },
                order);
        }

    }
}



