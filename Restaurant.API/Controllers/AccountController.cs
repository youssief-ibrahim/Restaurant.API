using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Restaurant.Application.DTOS.Auth;
using Restaurant.Infrastructure.Services.Account;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IStringLocalizer<AccountController> localizer;
        public AccountController(IAccountService accountService, IStringLocalizer<AccountController> localizer)
        {
            this.accountService = accountService;
            this.localizer = localizer;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            await accountService.Register(registerDTO);
            return Ok($"{localizer["accountCreatedCheckEmail"].Value} \"{registerDTO.Email}\" .");
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail( string email,  string code)
        {
            await accountService.Confirmmail(email, code);
            return Ok(localizer["confirmemail"].Value);
        }
        [HttpPost("resend-code")]
        public async Task<IActionResult> ResendCode( string email)
        {
            await accountService.ResendCode(email);
            return Ok($"{localizer["confirmationCodeResent"].Value} \"{email}\"");
        }
        [HttpPost("change-email")]
        [Authorize]
        public async Task<IActionResult> ChangeEmail(string newemail)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userid)) return Unauthorized();

            await accountService.ChangeEmail(userid, newemail);
            return Ok($"{localizer["confirmationCodeResent"].Value} {newemail}");
        }
        [HttpPost("confirm-email-change")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmailChange( string code)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userid)) return Unauthorized();

            await accountService.ConfirmEmailChange(userid, code);
            return Ok(localizer["emailChangeSuccess"].Value);
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword( string currentPassword, string newpassword)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userid)) return Unauthorized();
            await accountService.changePassword(userid, currentPassword, newpassword);
            return Ok(localizer["passwordChangedSuccessfully"].Value);
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            await accountService.ForgetPassword(Email);
            return Ok($"{localizer["passwordResetCodeSent"].Value} {Email}");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword( string Email,  string code, string NewPassword)
        {
            await accountService.ResetPassword(Email, code, NewPassword);
            return Ok(localizer["passwordResetSuccess"].Value);
        }
    }
}
