using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Auth
{
    public class RegisterDTO
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
