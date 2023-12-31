﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NorthWind.DataDTO;
using System.Data;

namespace NorthWind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperController : ControllerBase
    {
        private readonly string _connectionString;

        public ShipperController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
                    return Ok(shippers);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ShipperDTO> GetShipperById(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string query = "SELECT * FROM Shippers WHERE ShipperId = @Id";
                var shipper = dbConnection.QueryFirstOrDefault<ShipperDTO>(query, new { Id = id });

                if (shipper == null)
                {
                    return NotFound();
                }

                return Ok(shipper);
            }
        }

        [HttpPost]
        public ActionResult<ShipperDTO> CreateShipper(ShipperDTO shipper)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string insertQuery = "INSERT INTO Shippers (ShipperName, Phone) VALUES (@ShipperName, @Phone)";

                try
                {
                    dbConnection.Execute(insertQuery, shipper);
                    return Ok("Shipper created successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public ActionResult<ShipperDTO> UpdateShipper(int id, ShipperDTO shipper)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string updateQuery = "UPDATE Shippers SET ShipperName = @ShipperName, Phone = @Phone WHERE ShipperId = @Id";

                try
                {
                    dbConnection.Execute(updateQuery, new { ShipperName = shipper.ShipperName, Phone = shipper.Phone, Id = id });
                    return Ok("Shipper updated successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteShipper(int id)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                string deleteQuery = "DELETE FROM Shippers WHERE ShipperId = @Id";

                try
                {
                    dbConnection.Execute(deleteQuery, new { Id = id });
                    return Ok("Shipper deleted successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
