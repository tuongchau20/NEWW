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
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly string? _connectionString;
        private readonly testContext _context;

        public SupplierController(IConfiguration configuration, testContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<SupplierDTO>> GetAllSupplier()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM OrderDetails";

                try
                {
                    IEnumerable<SupplierDTO> orderDetails = dbConnection.Query<SupplierDTO>(query);
                    Log.Information("Retrieved Supplier from the database.");
                    var rs = new Response<SupplierDTO>
                    {
                        status = 200,
                        message = "Successfully",
                        data = orderDetails
                    };
                    return Ok(rs);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while retrieving Supplier from the database.");
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet("{id}")]
        public ActionResult<SupplierDTO> GetSupplier(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Suppliers WHERE SupllierId = @Id";
                var supplier = dbConnection.QueryFirstOrDefault<SupplierDTO>(query, new { Id = id });

                if (supplier == null)
                {
                    return NotFound();
                }

                return Ok(supplier);
            }
        }
        [HttpPost]
        public ActionResult<SupplierDTO> PostSupplier(SupplierDTO customer)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string insertQuery = "INSERT INTO Suppliers (SupplierName,ContactName,Address,City,PostalCode,Country) VALUES (@SupplierName, @ContactName,@Address,@City, @PostalCode,@Country)";

                try
                {
                    dbConnection.Execute(insertQuery, customer);
                    Log.Information("Created a new Supplier: {SupplierName}", customer.SupplierName);
                    var rs = new Response<CategoryDTO>
                    {
                        status = 200,
                        message = "Successfully",
                    };
                    return Ok(rs);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while creating a new Supplier: {SupplierName}", customer.SupplierName);
                    return BadRequest(ex.Message);
                }
            }
           
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuppier(int id, SupplierDTO supplier)
        {
            var suppliers = await _context.Suppliers.FindAsync(id);
            if (suppliers == null)
            {
                return NotFound();
            }
            await _context.Suppliers.Where(p => p.SupplierId == id).ExecuteUpdateAsync(p => p
            .SetProperty(x => x.SupplierName, x => supplier.SupplierName)
            .SetProperty(x => x.ContactName, x => supplier.ContactName)
            .SetProperty(x => x.Address, x => supplier.Address)
            .SetProperty(x => x.City, x => supplier.City)
            .SetProperty(x => x.PostalCode, x => supplier.PostalCode)
            .SetProperty(x => x.Country, x => supplier.Country)


            );

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            var cate = await _context.Suppliers.Where(c => c.SupplierId == id).ExecuteDeleteAsync();
            return Ok();
        }
    }

}
