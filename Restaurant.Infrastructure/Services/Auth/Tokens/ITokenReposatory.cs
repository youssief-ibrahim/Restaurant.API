using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Restaurant.Infrastructure.Identity;

namespace Restaurant.Infrastructure.Services.Auth.Tokens
{
    public interface ITokenReposatory
    {
        Task<string> GenerateJwtToken(AppilcationUser user, UserManager<AppilcationUser> userManager, RoleManager<ApplicationRole> roleManager);
        RefreshToken GenerateRefreshToken();
        Task<bool> RevokeRefreshTokenAsync(string token, UserManager<AppilcationUser> userManager);
    }
}
