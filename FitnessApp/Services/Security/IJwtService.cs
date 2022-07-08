using FitnessApp.Models;
using Microsoft.IdentityModel.Tokens;

namespace FitnessApp.Services.Security
{
    public interface IJwtService
    {
        SecurityToken GenerateToken(ApplicationUser user, bool isInAdminRole);
    }
}
