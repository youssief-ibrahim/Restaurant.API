using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Auth
{
    public class TokenResnoseDTO
    {
        public bool IsAuthanticated { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }
}
