using NorthWind.Models;

namespace Northwind.DTO
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public CategoryDTO()
        {
            // Default constructor
        }

        public CategoryDTO(Category sourceCategory)
        {
            CategoryId = sourceCategory.CategoryId;
            CategoryName = sourceCategory.CategoryName;
            Description = sourceCategory.Description;
        }
    }
}
