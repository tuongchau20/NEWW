using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthWind.Models;
using NorthWind.Repositories;

namespace NorthWind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _repo;

        public AccountsController(IAccountRepository repo)
        {
            _repo = repo;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel signUpmodel)
        {
            var rs = await _repo.SignUpAssync(signUpmodel);
            if(rs.Succeeded)
            {
                return Ok(rs.Succeeded);
            }
            return Unauthorized();
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> Signin(SignInModel signInModel)
        {
            var rs = await _repo.SignInAssync(signInModel);
            if(string.IsNullOrEmpty(rs))
            {
                return Unauthorized();
            }
            return Ok(rs);
        }
    }
}
