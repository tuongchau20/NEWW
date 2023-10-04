using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NorthWind.DataDTO;
using NorthWind.Helpers;
using NorthWind.Models;
using Serilog;
using System.Data;

namespace NorthWind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperController : ControllerBase
    {

        private readonly string? _connectionString;
        private readonly testContext _context;

        public ShipperController(IConfiguration configuration, testContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<ShipperDTO>> GetAllShippers()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Shippers";

                try
                {

                    IEnumerable<ShipperDTO> shippers = dbConnection.Query<ShipperDTO>(query);
                    Log.Information("Retrieved shippers from the database.");
                    var rs = new Response<ShipperDTO>
                    {
                        status = 200,
                        message = "Successfully",
                        data = shippers
                    };
                    return Ok(rs);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while retrieving shippers from the database.");
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
