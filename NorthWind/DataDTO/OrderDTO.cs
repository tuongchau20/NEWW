using NorthWind.Models;

namespace NorthWind.DataDTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? ShipperId { get; set; }
        public CustomerDTO Customer { get; set; }
        public EmployeeDTO Employee { get; set; }
        public ShipperDTO Shipper { get; set; }
        public ICollection<OrderDetailDTO> Details { get; set; }

        public OrderDTO()
        {
            // Default constructor
        }

        public OrderDTO(Order sourceOrder)
        {
            OrderId = sourceOrder.OrderId;
            CustomerId = sourceOrder.CustomerId;
            EmployeeId = sourceOrder.EmployeeId;
            OrderDate = sourceOrder.OrderDate;
            ShipperId = sourceOrder.ShipperId;

            
            Customer = new CustomerDTO(sourceOrder.Customer);
            Employee = new EmployeeDTO(sourceOrder.Employee);
            Shipper = new ShipperDTO(sourceOrder.Shipper);

            Details = sourceOrder.OrderDetails.Select(detail => new OrderDetailDTO(detail)).ToList();
        }
    }
}
