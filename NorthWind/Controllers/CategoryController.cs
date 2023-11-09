using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Northwind.DTO;
using NorthWind.Helpers;
using NorthWind.Models;
using Serilog;
using System.Data;

public interface ICategoryController
{
    ActionResult<IEnumerable<CategoryDTO>> GetAllCategories();
    ActionResult<CategoryDTO> GetByIdCategory(int id);
    Task<IActionResult> newDeleteCategory(int id);
    ActionResult<CategoryDTO> PostCategory(CategoryDTO category);
    Task<IActionResult> PutCategory(int id, CategoryDTO category);
}
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase, ICategoryController
{
    private readonly string _connectionString;
    private readonly testContext _context;

    public CategoryController(IConfiguration configuration, testContext context)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _context = context;
    }
    //somechange
    // GET: api/Category
    [HttpGet]
    public ActionResult<IEnumerable<CategoryDTO>> GetAllCategories()
    {
        using (IDbConnection dbConnection = new SqlConnection(_connectionString))
        {
            dbConnection.Open();
            string query = "SELECT * FROM Categories";

            try
            {

                IEnumerable<CategoryDTO> categories = dbConnection.Query<CategoryDTO>(query);
                Log.Information("Retrieved categories from the database.");
                var rs = new Response<CategoryDTO>
                {
                    status = 200,
                    message = "Successfully",
                    data = categories
                };
                return Ok(rs);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while retrieving categories from the database.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }

    // GET: api/Category/5
    
    [HttpGet("{id}")]
    public ActionResult<CategoryDTO> GetByIdCategory(int id)
    {
        using (IDbConnection dbConnection = new SqlConnection(_connectionString))
        {
            dbConnection.Open();
            string query = "SELECT * FROM Categories WHERE CategoryId = @Id";
            var category = dbConnection.QueryFirstOrDefault<CategoryDTO>(query, new { Id = id });

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }

    // POST: api/Category
    [HttpPost]
    [Authorize]
    public ActionResult<CategoryDTO> PostCategory(CategoryDTO category)
    {
        using (IDbConnection dbConnection = new SqlConnection(_connectionString))
        {
            dbConnection.Open();
            string insertQuery = "INSERT INTO Categories (CategoryName, Description) VALUES (@CategoryName, @Description)";

            try
            {
                dbConnection.Execute(insertQuery, category);
                Log.Information("Created a new category: {CategoryName}", category.CategoryName);
                var rs = new Response<CategoryDTO>
                {
                    status = 200,
                    message = "Successfully",
                };
                return Ok(rs);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while creating a new category: {CategoryName}", category.CategoryName);
                return BadRequest(ex.Message);
            }
        }
    }

    // PUT: api/Category/5
    //public IActionResult PutCategory(int id, CategoryDTO category)
    //{
    //    if (id != category.CategoryId)
    //    {
    //        Log.Information("Invalid request: CategoryId in the URL does not match the category data.");
    //        return BadRequest();
    //    }

    //    using (IDbConnection dbConnection = new SqlConnection(_connectionString))
    //    {
    //        dbConnection.Open();
    //        string updateQuery = "UPDATE Categories SET CategoryName = @CategoryName, Description = @Description WHERE CategoryId = @CategoryId";

    //        try
    //        {
    //            int affectedRows = dbConnection.Execute(updateQuery, category);

    //            if (affectedRows == 0)
    //            {
    //                Log.Information($"Category with ID {id} not found.");
    //                return NotFound();
    //            }

    //            Log.Information($"Updated category with ID {id}");
    //            return NoContent();
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error(ex, $"Error while updating the category with ID {id}");
    //            return StatusCode(500, "An error occurred while processing your request.");
    //        }
    //    }
    //}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, CategoryDTO category)
    {
        var categories = await _context.Categories.FindAsync(id);
        if (categories == null)
        {
            return NotFound();
        }
        await _context.Categories.Where(p => p.CategoryId == id).ExecuteUpdateAsync(p => p
        .SetProperty(x => x.CategoryName, x => category.CategoryName)
        .SetProperty(x => x.Description, x => category.Description));
        return Ok();
    }
    //DELETE: api/Category/5
    //    [HttpDelete("{id}")]
    //    public IActionResult DeleteCategory(int id)
    //    {
    //        using (IDbConnection dbConnection = new SqlConnection(_connectionString))
    //        {
    //            dbConnection.Open();
    //            string deleteQuery = "DELETE FROM Categories WHERE CategoryId = @Id";

    //            try
    //            {
    //                int affectedRows = dbConnection.Execute(deleteQuery, new { Id = id });

    //                if (affectedRows == 0)
    //                {
    //                    Log.Information($"Category with ID {id} not found.");
    //                    return NotFound();
    //                }

    //                Log.Information($"Deleted category with ID {id}");
    //                var rs = new Response<CategoryDTO>
    //                {
    //                    status = 200,
    //                    message = "Successfully",
    //                };
    //                return Ok(rs);
    //            }
    //            catch (Exception ex)
    //            {
    //                Log.Error(ex, $"Error while deleting the category with ID {id}");
    //                return BadRequest(ex.Message);
    //            }
    //        }
    //    }
    //}//
    [HttpDelete("{id}")]
    public async Task<IActionResult> newDeleteCategory(int id)
    {
        var categories = await _context.Categories.FindAsync(id);
        if (categories == null)
        {
            return NotFound();
        }

        var cate = await _context.Categories.Where(c => c.CategoryId == id).ExecuteDeleteAsync();
        return Ok();
    }
}    