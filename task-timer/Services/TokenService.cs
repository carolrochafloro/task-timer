using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using task_timer.Serices;

namespace task_timer.Services;

public class TokenService : ITokenService
{
    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims,
                            IConfiguration _configuration)
    {
        // get secret key
        var key = _configuration.GetSection("JWT").GetValue<string>("SecretKey") ??
                  throw new InvalidOperationException("Invalid secret key");
        // convert string key to bytes
        var privateKey = Encoding.UTF8.GetBytes(key);

        // create signin credentials useing hmacsha256 to encrypt
        var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey),
                                SecurityAlgorithms.HmacSha256Signature);

        // set parameters to create token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetSection("JWT").
                      GetValue<double>("TokenValidityInMinutes")),
            Audience = _configuration.GetSection("JWT").GetValue<string>("ValidAudience"),
            Issuer = _configuration.GetSection("JWT").GetValue<string>("ValidIssuer"),
            SigningCredentials = signinCredentials,

        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return token;
    }

    public string GenerateRefreshToken()
    {
        var secureRandonBytes = new Byte[128];
        using var randomNumber = RandomNumberGenerator.Create();
        randomNumber.GetBytes(secureRandonBytes);

        var refreshToken = Convert.ToBase64String(secureRandonBytes);
        return refreshToken;
    }

    public ClaimsPrincipal GetPrincipalExpiredToken(string token, IConfiguration _configuration)
    {
        // receives expired token, get claims and return them

        var secretKey = _configuration["JWT:SecretKey"] ??
                        throw new InvalidOperationException("Invalid Key");

        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParams,
                        out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
            {
            throw new InvalidOperationException("Invalid token");
            }

        return principal;

    }
}
