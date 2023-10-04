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
    public interface IEmployeeController
    {
        Task<IActionResult> DeleteEmployee(int id);
        ActionResult<IEnumerable<EmployeeDTO>> GetAllEmployees();
        ActionResult<EmployeeDTO> GetEmployeeById(int id);
        ActionResult<EmployeeDTO> PostCustomer(EmployeeDTO employee);
        Task<IActionResult> PutEmployee(int id, EmployeeDTO employee);
    }

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase, IEmployeeController
    {
        private readonly string? _connectionString;
        private readonly testContext _context;

        public EmployeeController(IConfiguration configuration, testContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<EmployeeDTO>> GetAllEmployees()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Employees";

                try
                {

                    IEnumerable<EmployeeDTO> employees = dbConnection.Query<EmployeeDTO>(query);
                    Log.Information("Retrieved employees from the database.");
                    var rs = new Response<EmployeeDTO>
                    {
                        status = 200,
                        message = "Successfully",
                        data = employees
                    };
                    return Ok(rs);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while retrieving employees from the database.");
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet("{id}")]
        public ActionResult<EmployeeDTO> GetEmployeeById(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Employees WHERE EmployeeId = @Id";
                var employee = dbConnection.QueryFirstOrDefault<EmployeeDTO>(query, new { Id = id });

                if (employee == null)
                {
                    return NotFound();
                }

                return Ok(employee);
            }
        }
        [HttpPost]
        public ActionResult<EmployeeDTO> PostCustomer(EmployeeDTO employee)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string insertQuery = "INSERT INTO Employees (LastName,FirstName,BirthDate,Photo,Notes) VALUES (@LastName, @FirstName,@BirthDate, @Photo,@Notes)";

                try
                {
                    dbConnection.Execute(insertQuery, employee);
                    Log.Information("Created a new employee: {LastName}", employee.LastName);
                    var rs = new Response<EmployeeDTO>
                    {
                        status = 200,
                        message = "Successfully",
                    };
                    return Ok(rs);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while creating a new employee: {LastName}", employee.LastName);
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, EmployeeDTO employee)
        {
            var employees = await _context.Customers.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }
            await _context.Employees.Where(p => p.EmployeeId == id).ExecuteUpdateAsync(p => p
            .SetProperty(x => x.LastName, x => employee.LastName)
            .SetProperty(x => x.FirstName, x => employee.FirstName)
            .SetProperty(x => x.BirthDate, x => employee.BirthDate)
            .SetProperty(x => x.Photo, x => employee.Photo)
            .SetProperty(x => x.Notes, x => employee.Notes)
            );

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }

            var cate = await _context.Employees.Where(c => c.EmployeeId == id).ExecuteDeleteAsync();
            return Ok();
        }
    }
}
