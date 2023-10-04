using NorthWind.Models;

namespace NorthWind.DataDTO
{
    public class SupplierDTO
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }

        public SupplierDTO()
        {
            // Default constructor
        }

        public SupplierDTO(Supplier sourceSupplier)
        {
            SupplierId = sourceSupplier.SupplierId;
            SupplierName = sourceSupplier.SupplierName;
            ContactName = sourceSupplier.ContactName;
            Address = sourceSupplier.Address;
            City = sourceSupplier.City;
            PostalCode = sourceSupplier.PostalCode;
            Country = sourceSupplier.Country;
            Phone = sourceSupplier.Phone;
        }
    }
}
