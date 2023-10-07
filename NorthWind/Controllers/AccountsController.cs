using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NorthWind.Models.account;
using NorthWind.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NorthWind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountsController(IAccountRepository repo , UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _repo = repo;
            _userManager = userManager;
            _configuration = configuration;

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
            if (string.IsNullOrEmpty(rs))
            {
                return Unauthorized();
            }
            return Ok(rs);

        }
        [HttpPost("refresh")]
        [Authorize] // Đảm bảo rằng chỉ người dùng đã xác thực mới có thể sử dụng API này
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                // Lấy thông tin người dùng từ User.Identity.Name hoặc các thông tin khác có thể đại diện cho người dùng
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                if (IsValidRefreshToken(user.RefreshToken, user.RefreshTokenValidity))
                {
                    var accessToken = GenerateAccessToken(user);
                    return Ok(new { AccessToken = accessToken });
                }
                else
                {
                    return Unauthorized("Invalid or expired refresh token.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private bool IsValidRefreshToken(string refreshToken, DateTime refreshTokenValidity)
        {
            // Kiểm tra tính hợp lệ của refresh token dựa trên quy tắc của bạn, ví dụ: so sánh với thời gian hiện tại
            return !string.IsNullOrEmpty(refreshToken) && refreshTokenValidity >= DateTime.UtcNow;
        }

        private string GenerateAccessToken(ApplicationUser user)
        {
            var authClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(5), // Thời gian hết hạn của access token
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}
