using Jose;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NorthWind.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NorthWind.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signinmanager;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtsettings;

        public AccountRepository(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        { 
            _usermanager = userManager;
            _signinmanager = signInManager;
            _configuration = configuration;
            _jwtsettings = jwtSettings;
        }
        public async Task<string> SignInAssync(SignInModel model)
        {
            var result = await _signinmanager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (!result.Succeeded)
            {
                return null; // Đăng nhập không thành công
            }

            var user = await _usermanager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return null; // Không tìm thấy người dùng
            }

            // Tạo refresh token
            var refreshToken = Guid.NewGuid().ToString();

            // Lưu trữ refresh token và thời gian hết hạn trong cơ sở dữ liệu
            user.RefreshToken = refreshToken;
            user.RefreshTokenValidity = DateTime.UtcNow.AddMinutes(1440); // Ví dụ: hết hạn sau 1 ngày

            await _usermanager.UpdateAsync(user);

            // Tạo access token
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, model.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(20), // Thời gian hết hạn của access token
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<IdentityResult> SignUpAssync(SignUpModel model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };
            return await _usermanager.CreateAsync(user, model.Password);
        }

    }
}
