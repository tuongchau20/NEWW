using NorthWind.Models;

namespace NorthWind.DataDTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public string Unit { get; set; }
        public decimal? Price { get; set; }

        public ProductDTO(ProductDTO product)
        {
        }

        public ProductDTO(Product sourceProduct)
        {
            ProductId = sourceProduct.ProductId;
            ProductName = sourceProduct.ProductName;
            SupplierId = sourceProduct.SupplierId;
            CategoryId = sourceProduct.CategoryId;
            Unit = sourceProduct.Unit;
            Price = sourceProduct.Price;
        }
    }
}
