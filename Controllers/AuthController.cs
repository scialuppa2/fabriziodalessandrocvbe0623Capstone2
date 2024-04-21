using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Since1999.Models;
using Since1999.Services;

namespace Since1999.Controllers
{
    public class AuthController : ApiController
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;
        private readonly UserService _userService;

        public AuthController()
        {
            _secretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSettings:SecretKey"];
            _issuer = System.Configuration.ConfigurationManager.AppSettings["JwtSettings:Issuer"];
            _audience = System.Configuration.ConfigurationManager.AppSettings["JwtSettings:Audience"];
            _expirationMinutes = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["JwtSettings:ExpirationMinutes"]);

            _userService = new UserService();
        }

        [HttpPost]
        [Route("api/auth/login")]
        public IHttpActionResult Login([FromBody] LoginModel login)
        {
            if (_userService.IsValidUser(login.Email, login.Password))
            {
                var tokenString = GenerateJwtToken(login.Email);
                return Ok(new { token = tokenString });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890qwertyuiopasdfghjklzxcvbnm"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_expirationMinutes);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Aggiungi un log per verificare il token generato
            Console.WriteLine("Token generato: " + tokenString);

            return tokenString;
        }

    }
}
