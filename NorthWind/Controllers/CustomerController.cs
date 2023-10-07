using Dapper;
using Microsoft.AspNetCore.Http;
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
    public interface ICustomerController
    {
        Task<IActionResult> DeleteCustomer(int id);
        ActionResult<IEnumerable<CustomerDTO>> GetAllCustomers();
        ActionResult<CustomerDTO> GetCustomerById(int id);
        ActionResult<CustomerDTO> PostCustomer(CustomerDTO customer);
        Task<IActionResult> PutCustomer(int id, CustomerDTO customer);
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase, ICustomerController
    {
        private readonly string _connectionString;
        private readonly testContext _context;

        public CustomerController(IConfiguration configuration, testContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<CustomerDTO>> GetAllCustomers()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Customers";

                try
                {

                    IEnumerable<CustomerDTO> customers = dbConnection.Query<CustomerDTO>(query);
                    Log.Information("Retrieved customers from the database.");
                    var rs = new Response<CustomerDTO>
                    {
                        status = 200,
                        message = "Successfully",
                        data = customers
                    };
                    return Ok(rs);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while retrieving customers from the database.");
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet("{id}")]
        public ActionResult<CustomerDTO> GetCustomerById(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Customers WHERE CustomerId = @Id";
                var customer = dbConnection.QueryFirstOrDefault<CustomerDTO>(query, new { Id = id });

                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(customer);
            }
        }
        // POST: api/customer
        [HttpPost]
        public ActionResult<CustomerDTO> PostCustomer(CustomerDTO customer)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string insertQuery = "INSERT INTO Customers (CustomerName,ContactName,Address,City,PostalCode,Country) VALUES (@CustomerName, @ContactName,@Address,@City, @PostalCode,@Country)";

                try
                {
                    dbConnection.Execute(insertQuery, customer);
                    Log.Information("Created a new customer: {CustomerName}", customer.CustomerName);
                    var rs = new Response<CategoryDTO>
                    {
                        status = 200,
                        message = "Successfully",
                    };
                    return Ok(rs);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while creating a new customer: {CustomerName}", customer.CustomerName);
                    return BadRequest(ex.Message);
                }
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDTO customer)
        {
            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            await _context.Customers.Where(p => p.CustomerId == id).ExecuteUpdateAsync(p => p
            .SetProperty(x => x.CustomerName, x => customer.CustomerName)
            .SetProperty(x => x.ContactName, x => customer.ContactName)
            .SetProperty(x => x.Address, x => customer.Address)
            .SetProperty(x => x.City, x => customer.City)
            .SetProperty(x => x.PostalCode, x => customer.PostalCode)
            .SetProperty(x => x.Country, x => customer.Country)
            );

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }

            var cate = await _context.Customers.Where(c => c.CustomerId == id).ExecuteDeleteAsync();
            return Ok();
        }
    }
}
