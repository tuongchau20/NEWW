using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using NorthWind.DataDTO;
using NorthWind.Models;
using Microsoft.EntityFrameworkCore;

namespace NorthWind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly testContext _context;

        public OrderController(IConfiguration configuration, testContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;


        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderDTO>> GetAllOrders()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Orders";

                try
                {
                    IEnumerable<OrderDTO> orders = dbConnection.Query<OrderDTO>(query);
                    return Ok(orders);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("{id}")]
        public ActionResult<OrderDTO> GetOrderById(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Orders WHERE OrderId = @Id";
                var order = dbConnection.QueryFirstOrDefault<OrderDTO>(query, new { Id = id });

                if (order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }
        }

        [HttpPost]
        public ActionResult<OrderDTO> CreateOrder(OrderDTO order)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string insertQuery = "INSERT INTO Orders (CustomerId, EmployeeId, OrderDate, ShipperId) VALUES (@CustomerId, @EmployeeId, @OrderDate, @ShipperId)";

                try
                {
                    dbConnection.Execute(insertQuery, order);
                    return Ok("Order created successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

   

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDTO order)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            await _context.Orders.Where(p => p.OrderId == id).ExecuteUpdateAsync(p => p
            .SetProperty(x => x.CustomerId, x => orders.CustomerId)
            .SetProperty(x => x.EmployeeId, x => orders.EmployeeId)
            .SetProperty(x => x.OrderDate, x => orders.OrderDate)
            .SetProperty(x => x.ShipperId, x => orders.ShipperId)

            );

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }

            var cate = await _context.Orders.Where(c => c.OrderId == id).ExecuteDeleteAsync();
            return Ok();
        }
    }
}
