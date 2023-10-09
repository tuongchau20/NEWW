    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using NorthWind.Models.account;
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

            public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
            { 
                _usermanager = userManager;
                _signinmanager = signInManager;
                _configuration = configuration;
            }
        public async Task<string> SignInAssync(SignInModel model)
        {
            var result = await _signinmanager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (!result.Succeeded)
            {
                return null; 
            }

            var user = await _usermanager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return null; 
            }

            if (user.UserName == null)
            {
               user.UserName = "Unknown";
            }

            // Tạo refresh token
            var refreshToken = Guid.NewGuid().ToString();

            // Lưu trữ refresh token và thời gian hết hạn trong cơ sở dữ liệu
            user.RefreshToken = refreshToken;
            user.RefreshTokenValidity = DateTime.Now.AddMinutes(15); // Ví dụ: hết hạn sau 15 phút
            await _usermanager.UpdateAsync(user);
            // Tạo danh sách các Claim, bao gồm Claim về userName
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, model.Email),
        new Claim(ClaimTypes.Name, user.UserName), // Kiểm tra và thêm userName
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
