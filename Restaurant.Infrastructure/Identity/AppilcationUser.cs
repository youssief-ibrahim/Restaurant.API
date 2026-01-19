using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Restaurant.Domain.Models;

namespace Restaurant.Infrastructure.Identity
{
    public class AppilcationUser: IdentityUser<int>
    {
        public string Name { get; set; }
        public string? Code { get; set; }
        public string? RefrenceNewEmail { get; set; }
        public DateTime? CodeExpiry { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

        // Navigation Properties
        public Customer? Customer { get; set; }
        public Delivery? Delivery { get; set; }
        public Chef? Chef { get; set; }
    }
}
