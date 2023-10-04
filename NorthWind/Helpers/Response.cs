using Northwind.DTO;

namespace NorthWind.Helpers
{
    public class Response<T>
    {
        public int status { get; set; }
        public string message { get; set; }
        public IEnumerable<T> data { get; set; }
    }
}
