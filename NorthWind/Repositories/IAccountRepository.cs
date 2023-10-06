using Microsoft.AspNetCore.Identity;
using NorthWind.Models;

namespace NorthWind.Repositories
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> SignUpAssync(SignUpModel model);
        public Task<string> SignInAssync(SignInModel model);
    }
}
