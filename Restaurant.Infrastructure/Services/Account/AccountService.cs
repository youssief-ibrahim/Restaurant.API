using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Restaurant.Application.DTOS.Auth;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Exceptions;
using Restaurant.Infrastructure.Identity;

namespace Restaurant.Infrastructure.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IStringLocalizer<AccountService> localizer;
        private readonly UserManager<AppilcationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IEmailService emailService;

        public AccountService(IStringLocalizer<AccountService> localizer, UserManager<AppilcationUser> userManager, RoleManager<ApplicationRole> roleManager, IEmailService emailService)
        {
            this.localizer = localizer;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
        }
        public async Task Register(RegisterDTO registerDTO)
        {

            if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
                throw new BadRequestException(localizer["email"].Value);

            if (await userManager.FindByNameAsync(registerDTO.UserName) != null)
                throw new BadRequestException(localizer["username"].Value);

            if (!new EmailAddressAttribute().IsValid(registerDTO.Email))
                throw new BadRequestException(localizer["invalidemailformate"].Value);

            AppilcationUser user = new AppilcationUser()
            {
                Name = registerDTO.Name,
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
            };
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException(string.Join(", ", errors));
            }
            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");
            user.Code = code;
            user.CodeExpiry = DateTime.Now.AddMinutes(5);
            await userManager.UpdateAsync(user);

            await emailService.SendEmailAsync(
                Email: registerDTO.Email,
                subject: localizer["confirmationEmail"].Value,
                body: $"{localizer["confirmEmailCode"].Value} {code}"
                );
            await userManager.AddToRoleAsync(user, "Customer");
        }

        public async Task Confirmmail(string email, string code)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new NotFoundException(localizer["usernotfound"].Value);

            if (user.Code != code)
                throw new BadRequestException(localizer["invalidcode"].Value);

            if (user.CodeExpiry < DateTime.Now)
                throw new NotFoundException(localizer["codeexpired"].Value);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.Code = null;
                user.CodeExpiry = null;
                await userManager.UpdateAsync(user);
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException(string.Join(", ", errors));
            }
        }

        public async Task ResendCode(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new NotFoundException(localizer["usernotfound"].Value);

            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");
            user.Code = code;
            user.CodeExpiry = DateTime.Now.AddMinutes(5);
            await userManager.UpdateAsync(user);

            await emailService.SendEmailAsync(
                Email: email,
                subject: localizer["confirmyouremail"].Value,
                body: $"{localizer["confirmEmailCode"].Value} {code}"
            );

        }

        public async Task ChangeEmail(string userid, string newemail)
        {

            var user = await userManager.FindByIdAsync(userid);

            if (user == null)
                throw new NotFoundException(localizer["usernotfound"].Value);

            var existingUser = await userManager.FindByEmailAsync(newemail);
            if (existingUser != null)
                throw new BadRequestException(localizer["email"].Value);

            user.Email = newemail;

            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");
            user.Code = code;
            user.CodeExpiry = DateTime.Now.AddMinutes(5);
            user.RefrenceNewEmail = newemail;
            await userManager.UpdateAsync(user);

            await emailService.SendEmailAsync(
                Email: newemail,
                subject: localizer["confirmyouremail"].Value,
                body: $"{localizer["confirmEmailCode"].Value} {code}"
             );
        }

        public async Task ConfirmEmailChange(string userid, string code)
        {

            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
                throw new NotFoundException(localizer["usernotfound"].Value);

            if (user.Code != code)
                throw new BadRequestException(localizer["invalidcode"].Value);

            if (user.CodeExpiry < DateTime.Now)
                throw new BadRequestException(localizer["codeexpired"].Value);

            var newmail = user.RefrenceNewEmail;
            var token = await userManager.GenerateChangeEmailTokenAsync(user, newmail);
            var result = await userManager.ChangeEmailAsync(user, newmail, token);
            if (result.Succeeded)
            {
                user.Code = null;
                user.RefrenceNewEmail = null;
                user.CodeExpiry = null;
                await userManager.UpdateAsync(user);
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException(string.Join(", ", errors));
            }
        }

        public async Task changePassword(string userid, string currentPassword, string newpassword)
        {
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
                throw new NotFoundException(localizer["usernotfound"].Value);

            if (currentPassword == newpassword)
                throw new BadRequestException("new password must be diffrent from current password");

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newpassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException(string.Join(", ", errors));
            }
        }

        public async Task ForgetPassword(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null) throw new NotFoundException(localizer["usernotfound"].Value);

            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");
            user.Code = code;
            user.CodeExpiry = DateTime.Now.AddMinutes(5);
            await userManager.UpdateAsync(user);
            await emailService.SendEmailAsync(
                Email: Email,
                subject: localizer["resetPasswordCodeTitle"].Value,
                body: $"{localizer["resetPasswordCodeMessage"].Value} {code}"
                );
        }

        public async Task ResetPassword(string Email, string code, string NewPassword)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
                throw new NotFoundException(localizer["usernotfound"].Value);
            if (user.Code != code)
                throw new BadRequestException("Invalid code");

            if (user.CodeExpiry < DateTime.Now)
                throw new BadRequestException("Code expired");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, NewPassword);

            if (result.Succeeded)
            {
                user.Code = null;
                user.CodeExpiry = null;
                await userManager.UpdateAsync(user);
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new BadRequestException(string.Join(", ", errors));
            }

        }
    }
}
