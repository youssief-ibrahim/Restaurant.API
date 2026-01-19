using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Restaurant.Application.DTOS.Auth;
using Restaurant.Domain.Exceptions;
using Restaurant.Infrastructure.Identity;
using Restaurant.Infrastructure.Services.Auth.Tokens;

namespace Restaurant.Infrastructure.Services.Auth.AuthService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppilcationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ITokenReposatory tokenService;
        private readonly IStringLocalizer<AuthenticationService> localizer;

        public AuthenticationService(UserManager<AppilcationUser> userManager, RoleManager<ApplicationRole> roleManager, ITokenReposatory tokenService, IStringLocalizer<AuthenticationService> localizer)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
            this.localizer = localizer;
        }
        public async Task<TokenResnoseDTO> Login(LoginDTO dto)
        {
            var user = await userManager.FindByNameAsync(dto.UserName);

            if (user == null)
                throw new BadHttpRequestException(localizer["invalidusername"].Value);

            if (!await userManager.CheckPasswordAsync(user, dto.Password))
                throw new BadHttpRequestException(localizer["invalidpassword"].Value);
            if (!user.EmailConfirmed)
                throw new BadHttpRequestException(localizer["emailnotconfirmed"].Value);

            // Generate access token
            var accessToken = await tokenService.GenerateJwtToken(user, userManager, roleManager);

            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);


            return new TokenResnoseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpires = refreshToken.ExpireOn,
                IsAuthanticated = true
            };

        }
        // remember me 
        public async Task<TokenResnoseDTO> RememberMe(string refreshToken)
        {
            var user = await userManager.Users
                .Include(x => x.RefreshTokens)
                .SingleOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == refreshToken));
            if (user == null)
                throw new BadHttpRequestException(localizer["invalidrefreshtoken"].Value);
            var storedToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if (storedToken == null || !storedToken.IsActive)
                throw new BadHttpRequestException(localizer["invalidorrevokedrefreshtoken"].Value);
            var accessToken = await tokenService.GenerateJwtToken(user, userManager, roleManager); // rember use await

            return new TokenResnoseDTO
            {
                Token = accessToken,
                RefreshToken = storedToken.Token,
                RefreshTokenExpires = storedToken.ExpireOn,
                IsAuthanticated = true
            };
        }
        public async Task Logout(string refreshToken)
        {

            var result = await tokenService.RevokeRefreshTokenAsync(refreshToken, userManager);

            if (!result)
                throw new BadRequestException(localizer["tokeninvalidoralreadyrevoked"].Value);

        }
        public async Task<TokenResnoseDTO> RefreshToken(string refreshToken)
        {
            var user = await userManager.Users
                .Include(x => x.RefreshTokens)
                .SingleOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
                throw new BadRequestException(localizer["invalidrefreshtoken"].Value);

            var storedToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                throw new BadRequestException(localizer["invalidorrevokedrefreshtoken"].Value);

            // 🔥 If NOT EXPIRED → use it (DO NOT create new)
            if (storedToken.ExpireOn > DateTime.Now)
            {
                var newAccessToken = await tokenService.GenerateJwtToken(user, userManager, roleManager); // remember use await

                return new TokenResnoseDTO
                {
                    Token = newAccessToken,
                    RefreshToken = storedToken.Token,
                    RefreshTokenExpires = storedToken.ExpireOn,
                    IsAuthanticated = true
                };
            }

            // ❗ If EXPIRED → create a new refresh token
            var newRefresh = tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefresh);
            await userManager.UpdateAsync(user);

            var accessToken2 = await tokenService.GenerateJwtToken(user, userManager, roleManager); // remember use await

            return new TokenResnoseDTO
            {
                Token = accessToken2,
                RefreshToken = newRefresh.Token,
                RefreshTokenExpires = newRefresh.ExpireOn,
                IsAuthanticated = true
            };
        }


        public async Task RevokeToken(string token)
        {
            var result = await tokenService.RevokeRefreshTokenAsync(token, userManager);

            if (!result)
                throw new BadRequestException(localizer["tokeninvalidoralreadyrevoked"].Value);
        }

    }
}
