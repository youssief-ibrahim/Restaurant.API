using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Auth;
using Restaurant.Infrastructure.Identity;

namespace Restaurant.Infrastructure.Services.Auth.AuthService
{
    public interface IAuthenticationService
    {
        Task<TokenResnoseDTO> Login(LoginDTO dto);
        Task<TokenResnoseDTO> RememberMe(string refreshToken);
        Task Logout(string refreshToken);
        Task<TokenResnoseDTO> RefreshToken(string refreshToken);
        Task RevokeToken(string token);
    }
}
