using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Restaurant.Infrastructure.Identity;

namespace Restaurant.Infrastructure.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            // Add roles only if it doesn't exist

            var roles = new List<string> { "Admin", "Customer" , "Delivery" , "Chef" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    });
            }
        }
    }
}
