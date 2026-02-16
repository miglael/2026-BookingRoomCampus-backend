using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookingRoomCampus.Models;
//using Microsoft.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace BookingRoomCampus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        //Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            return Ok("User berhasil dibuat");
        }

        //Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return Unauthorized("Email atau password salah");
            }
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_MY_SUPER_SECRET_BOOKING_CAMPUS_KEY_2026_SECURE_123"));


            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

    }

}