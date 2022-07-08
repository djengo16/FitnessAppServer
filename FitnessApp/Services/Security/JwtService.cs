using FitnessApp.Common;
using FitnessApp.Models;
using FitnessApp.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessApp.Services.Security
{
    public class JwtService : IJwtService
    {
        private readonly IOptions<JwtSettings> jwtSettings;
        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            this.jwtSettings = jwtSettings;
        }

        public SecurityToken GenerateToken(ApplicationUser user, bool isInAdminRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.jwtSettings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                                 new SymmetricSecurityKey(key),
                                 SecurityAlgorithms.HmacSha512Signature)
            };

            if (isInAdminRole)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, GlobalConstants.AdministratorRoleName));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}