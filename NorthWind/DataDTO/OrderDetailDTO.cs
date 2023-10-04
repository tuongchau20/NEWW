using NorthWind.Models;

namespace NorthWind.DataDTO
{
    public class OrderDetailDTO
    {
        private OrderDetail detail;

        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public ProductDTO Product { get; set; }

        public OrderDetailDTO()
        {
        }

        public OrderDetailDTO(OrderDetailDTO sourceOrderDetail)
        {
            OrderDetailId = sourceOrderDetail.OrderDetailId;
            OrderId = sourceOrderDetail.OrderId;
            ProductId = sourceOrderDetail.ProductId;
            Quantity = sourceOrderDetail.Quantity;

            Product = new ProductDTO(sourceOrderDetail.Product);
        }

        public OrderDetailDTO(OrderDetail detail)
        {
            this.detail = detail;
        }
    }
}
