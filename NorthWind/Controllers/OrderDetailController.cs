using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Northwind.DTO;
using NorthWind.DataDTO;
using NorthWind.Helpers;
using NorthWind.Models;
using Serilog;
using System.Data;

namespace NorthWind.Controllers
{
    public interface IOrderDetailController
    {
        Task<IActionResult> DeleteOrderDetail(int id);
        ActionResult<IEnumerable<OrderDetailDTO>> GetAllOrderDetails();
        ActionResult<OrderDetailDTO> GetOrderDetailById(int id);
        ActionResult<OrderDetailDTO> PostOrderDetail(OrderDetailDTO orderDetail);
        Task<IActionResult> PutOrderDetail(int id, OrderDetailDTO orderDetail);
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase, IOrderDetailController
    {
        private readonly string _connectionString;
        private readonly testContext _context;

        public OrderDetailController(IConfiguration configuration, testContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderDetailDTO>> GetAllOrderDetails()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM OrderDetails";

                try
                {
                    IEnumerable<OrderDetailDTO> orderDetails = dbConnection.Query<OrderDetailDTO>(query);
                    Log.Information("Retrieved order details from the database.");
                    var rs = new Response<OrderDetailDTO>
                    {
                        status = 200,
                        message = "Successfully",
                        data = orderDetails
                    };
                    return Ok(rs);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while retrieving order details from the database.");
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("{id}")]
        public ActionResult<OrderDetailDTO> GetOrderDetailById(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM OrderDetails WHERE OrderDetailId = @Id";
                var orderDetail = dbConnection.QueryFirstOrDefault<OrderDetailDTO>(query, new { Id = id });

                if (orderDetail == null)
                {
                    return NotFound();
                }

                return Ok(orderDetail);
            }
        }

        [HttpPost]
        public ActionResult<OrderDetailDTO> PostOrderDetail(OrderDetailDTO orderDetail)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string insertQuery = "INSERT INTO OrderDetails (OrderId,ProductId,Quantity) VALUES (@OrderId, @ProductId,@Quantity)";

                try
                {
                    dbConnection.Execute(insertQuery, orderDetail);
                    Log.Information("Created a new OrderDetail: {OrderDetailId}", orderDetail.OrderDetailId);
                    var rs = new Response<CategoryDTO>
                    {
                        status = 200,
                        message = "Successfully",
                    };
                    return Ok(rs);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while creating a new OrderDetail: {OrderDetailId}", orderDetail.OrderDetailId);
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetailDTO orderDetail)
        {
            var orderdetails = await _context.OrderDetails.FindAsync(id);
            if (orderdetails == null)
            {
                return NotFound();
            }
            await _context.OrderDetails.Where(p => p.OrderId == id).ExecuteUpdateAsync(p => p
            .SetProperty(x => x.ProductId, x => orderdetails.ProductId)
            .SetProperty(x => x.Quantity, x => orderdetails.Quantity)



            );

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var customers = await _context.OrderDetails.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }

            var cate = await _context.OrderDetails.Where(c => c.OrderDetailId == id).ExecuteDeleteAsync();
            return Ok();
        }
    }
}
