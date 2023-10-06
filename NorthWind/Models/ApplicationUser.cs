using Microsoft.AspNetCore.Identity;

namespace NorthWind.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

    }
}
