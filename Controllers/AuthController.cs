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
            var user = _userService.GetUserByEmail(login.Email);
            if (user != null && _userService.IsValidUser(login.Email, login.Password))
            {
                var tokenString = GenerateJwtToken(login.Email, user); // Passa l'oggetto User al metodo GenerateJwtToken
                return Ok(new { token = tokenString, userId = user.UserID });
            }

            return Unauthorized();
        }



        private string GenerateJwtToken(string email, User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_expirationMinutes);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // Aggiungi altre informazioni sull'utente come claim aggiuntive
            new Claim("UserID", user.UserID.ToString()),
            new Claim("Nome", user.Nome),
            new Claim("Cognome", user.Cognome),
                    // Aggiungi altri claim personalizzati secondo necessità
                },
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }


    }
}
