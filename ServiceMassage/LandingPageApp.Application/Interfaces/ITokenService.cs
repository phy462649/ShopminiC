using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Account account);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
    }
}
