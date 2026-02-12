using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace QUANTM.Services.Auth
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual (string Token, string Jti) GenerateToken(int userId, string username, int roleId, string roleCode)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            // Priority: Environment Variable (Flat) > Configuration (appsettings/hierarchical env)
            var keyStr = Environment.GetEnvironmentVariable("JWT_KEY") ?? jwtSettings["Key"];
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings["Issuer"];
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings["Audience"];

            if (string.IsNullOrEmpty(keyStr))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            var key = Encoding.UTF8.GetBytes(keyStr);
            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim("role_id", roleId.ToString()),
                new Claim(ClaimTypes.Role, roleCode), // Crucial for security checks
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(240),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), jti);
        }
    }
}
