using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Auth;

namespace Restaurant.Infrastructure.Services.Account
{
    public interface IAccountService
    {
        Task Register(RegisterDTO registerDTO);
        Task Confirmmail(string email, string code);
        Task ResendCode(string email);
        Task ChangeEmail(string userid, string newemail);
        Task ConfirmEmailChange(string userid, string code);
        Task changePassword(string userid, string currentPassword, string newpassword);
        Task ForgetPassword(string Email);
        Task ResetPassword(string Email, string code, string NewPassword);
    }
}
