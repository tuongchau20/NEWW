using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using NorthWind.DataDTO;
using NorthWind.Models;

namespace NorthWind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly string _connectionString;

        public OrderController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
        public ActionResult<OrderDTO> UpdateOrder(int id, OrderDTO order)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string updateQuery = "UPDATE Orders SET CustomerId = @CustomerId, EmployeeId = @EmployeeId, OrderDate = @OrderDate, ShipperId = @ShipperId WHERE OrderId = @Id";

                try
                {
                    dbConnection.Execute(updateQuery, new { CustomerId = order.CustomerId, EmployeeId = order.EmployeeId, OrderDate = order.OrderDate, ShipperId = order.ShipperId, Id = id });
                    return Ok("Order updated successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteOrder(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string deleteQuery = "DELETE FROM Orders WHERE OrderId = @Id";

                try
                {
                    dbConnection.Execute(deleteQuery, new { Id = id });
                    return Ok("Order deleted successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
