using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace LandingPageApp.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSecret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? "LandingPageApp";
            _jwtAudience = _configuration["Jwt:Audience"] ?? "LandingPageAppUsers";
        }

        /// <summary>
        /// Generates an access token with 15-minute expiration
        /// </summary>
        public string GenerateAccessToken(Person person)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            Console.WriteLine(person);
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, person.Id.ToString()),
                new Claim(ClaimTypes.Name, person.Username),
                // Use "role" claim for ASP.NET Core authorization
                new Claim("role", person.Role.Name ),
                new Claim("Name", person.Name),
                new Claim("Email", person.Email ?? string.Empty),
                new Claim("Phone", person.Phone ?? string.Empty),
                new Claim("Address", person.Address ?? string.Empty)

            };
            Console.WriteLine(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a refresh token with 7-day expiration
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rand = RandomNumberGenerator.Create())
            {
                rand.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Validates a token and returns the claims principal
        /// </summary>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            Console.WriteLine(token);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret)),
                ValidateLifetime = false // Allow expired tokens for refresh
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        /// <summary>
        /// Validates a token without allowing expired tokens
        /// </summary>
        public bool ValidateToken(string token)
        {
         
            try{
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret)),
                    ValidateLifetime = true
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                Console.WriteLine(token);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token expired");
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                Console.WriteLine("Invalid signature");
                return false;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                Console.WriteLine("Invalid issuer");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation error: {ex.Message}");
                return false;
            }

        }
    }
}
