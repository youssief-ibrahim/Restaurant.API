using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Restaurant.Application.DTOS.Auth;
using Restaurant.Infrastructure.Identity;
using Restaurant.Infrastructure.Services.Auth.AuthService;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IStringLocalizer<AuthenticationController> localizer;
        public AuthenticationController(IAuthenticationService authenticationService, IStringLocalizer<AuthenticationController> localizer)
        {
            this.authenticationService = authenticationService;
            this.localizer = localizer;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await authenticationService.Login(dto);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpires);

            return Ok(result);
        }
        [HttpPost("remember-me")]
        public async Task<IActionResult> RememberMe()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("token is required");

            var result = await authenticationService.RememberMe(refreshToken);
            return Ok(result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("token is required");

            await authenticationService.Logout(refreshToken);

            Response.Cookies.Delete("refreshToken");

            return Ok(localizer["loggedoutsuccessfully"].Value);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            //  if i dont need string
            //var refreshToken = Request.Cookies["refreshToken"];
            //if (string.IsNullOrEmpty(refreshToken))                
            //    return BadRequest("token is required");
            var result = await authenticationService.RefreshToken(refreshToken);
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpires);
            return Ok(result);

        }
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(localizer["tokenisrequired"].Value);

            await authenticationService.RevokeToken(token);

            Response.Cookies.Delete("refreshToken");

            return Ok(localizer["tokenrevoked"].Value);
        }
        private void SetRefreshTokenInCookie(string token, DateTime expire)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expire
            };

            Response.Cookies.Append("refreshToken", token, options);
        }
    }
}
