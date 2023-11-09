using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NorthWind.Models.account;
using NorthWind.Repositories;
using System;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountRepository repo, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration ,ILogger<AccountsController> logger)
        {
            _repo = repo;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _logger = logger;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel signUpmodel)
        {
            var rs = await _repo.SignUpAssync(signUpmodel);
            if (rs.Succeeded)
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
        [Authorize]
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

                    //if (IsValidRefreshToken(user.RefreshToken, user.RefreshTokenValidity))
                    //{
                    //    var accessToken = GenerateAccessToken(user);
                    //    return Ok(new { AccessToken = accessToken });
                    //}
                    //else
                    //{
                    //    return Unauthorized("Invalid or expired refresh token.");
                    //} 
                    var accessToken = GenerateAccessToken(user);
                    return Ok(new { AccessToken = accessToken });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

        private bool IsValidRefreshToken(string refreshToken, DateTime refreshTokenValidity)
        {

            // Kiểm tra tính hợp lệ của refresh token dựa trên quy tắc của bạn, ví dụ: so sánh với thời gian hiện tại
            return !string.IsNullOrEmpty(refreshToken) && refreshTokenValidity >= DateTime.Now;
        }

        private JwtSecurityToken GenerateAccessToken(ApplicationUser user)
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
                    expires: DateTime.UtcNow.AddMinutes(5), // Thời gian hết hạn của access token
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                );
               
            return token;
        }
        [HttpPost("createadmin")]
        public async Task<IActionResult> CreateAdmin()
        {
            // Kiểm tra xem vai trò "Admin" đã tồn tại, nếu chưa thì bạn có thể tạo nó.
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                // Nếu vai trò "Admin" chưa tồn tại, thêm nó vào hệ thống.
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Tạo một tài khoản người dùng mới
            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };

            var result = await _userManager.CreateAsync(adminUser, "Tuong@123"); // Thay thế bằng mật khẩu thực tế

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                return Ok("Tài khoản admin đã được tạo thành công.");
            }
            else
            {
                return BadRequest("Có lỗi khi tạo tài khoản admin.");
            }
        }

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok("Role created successfully.");
            }

            return BadRequest("Role creation failed.");
        }

        [HttpPost("AddRoleToUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("Role does not exist.");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok("Role added to user successfully.");
            }

            return BadRequest("Role addition failed.");
        }
    }
}
