using NorthWind.Models;

namespace NorthWind.DataDTO
{
    public class ShipperDTO
    {
        public int ShipperId { get; set; }
        public string ShipperName { get; set; }
        public string Phone { get; set; }

        public ShipperDTO()
        {
        }

        public ShipperDTO(Shipper sourceShipper)
        {
            ShipperId = sourceShipper.ShipperId;
            ShipperName = sourceShipper.ShipperName;
            Phone = sourceShipper.Phone;
        }
    }
}
