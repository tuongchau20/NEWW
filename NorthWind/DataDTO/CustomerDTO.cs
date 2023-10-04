using NorthWind.Models;

namespace NorthWind.DataDTO
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public CustomerDTO()
        {

        }

        public CustomerDTO(Customer sourceCustomer)
        {
            CustomerId = sourceCustomer.CustomerId;
            CustomerName = sourceCustomer.CustomerName;
            ContactName = sourceCustomer.ContactName;
            Address = sourceCustomer.Address;
            City = sourceCustomer.City;
            PostalCode = sourceCustomer.PostalCode;
            Country = sourceCustomer.Country;
        }
    }
}
