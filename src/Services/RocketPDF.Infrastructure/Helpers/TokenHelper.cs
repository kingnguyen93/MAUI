using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RocketPDF.Shared.Helpers
{
    public static class TokenHelper
    {
        public static JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, string secret, string issuer, string audience, int accessExpiration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.Now;
            var expires = now.AddMinutes(accessExpiration);

            return new JwtSecurityToken(
                issuer,
                audience,
                claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials
            );
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string secret)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = false,
                //ValidIssuer = token.Issuer,
                ValidateAudience = false, // you might want to validate the audience and issuer depending on your use case
                //ValidAudience = token.Audience,
                ValidateLifetime = false // here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) == false)
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public static string GenerateOTP()
        {
            var otp = new StringBuilder();
            var rand = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 6; i++)
            {
                otp.Append(rand.Next(0, 9));
            }
            return otp.ToString();
        }
    }
}