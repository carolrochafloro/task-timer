using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace task_timer.Serices;

public interface ITokenService
{
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims,
                     IConfiguration _configuration);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalExpiredToken(string token, IConfiguration _configuration);

}
