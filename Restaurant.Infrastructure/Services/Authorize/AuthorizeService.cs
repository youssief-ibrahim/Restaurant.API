using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Restaurant.Application.DTOS.Authorize;
using Restaurant.Domain.Exceptions;
using Restaurant.Infrastructure.Identity;
using Restaurant.Infrastructure.Services.Auth.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Restaurant.Infrastructure.Services.Authorize
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly UserManager<AppilcationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ITokenReposatory tokenService;
        private readonly IStringLocalizer<AuthorizeService> localizer;
        public AuthorizeService(UserManager<AppilcationUser> userManager, RoleManager<ApplicationRole> roleManager, ITokenReposatory tokenService, IStringLocalizer<AuthorizeService> localizer)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
            this.localizer = localizer;
        }

        public async Task<RoleDTO> CreateAsync( CreateRoleDTO dto)
        {
            var checkrole=await roleManager.RoleExistsAsync(dto.Name);
            if (checkrole) throw new BadRequestException(localizer["roleexists"].Value);
            var role = new ApplicationRole
            {
                Name = dto.Name,
            };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException(localizer["rolecreationfailed"].Value + ": " + errors);
            }
            var admin = await userManager.FindByNameAsync("Admin");
            if (admin != null)  await userManager.AddToRoleAsync(admin, role.Name);
            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
            };
        }

        public async Task<bool> DeleteAsync( int id)
        {
            var role =await roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null) throw new NotFoundException(localizer["rolenotfound"].Value);
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
            var usersExceptAdmin = usersInRole.Where(u => u.UserName != "Admin"); // Cause admin get all roles
            if (usersExceptAdmin.Any())
            {
                throw new BadRequestException(localizer["roleinuse"].Value);
            }
            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException(localizer["roledeletefailed"].Value + ": " + errors);
            }
            return true;

        }

        public async Task<List<RoleDTO>> GetAllAsync()
        {
            var roles =await roleManager.Roles.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name,
            }).ToListAsync();
            return roles;
        }

        public async Task<RoleDTO> GetByIdAsync( int id)
        {
            var role =await roleManager.Roles.Where(r => r.Id == id).Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name,
            }).FirstOrDefaultAsync();
            if(role == null) throw new NotFoundException(localizer["rolenotfound"].Value);
            
            return role;
        }

        public async Task<RoleDTO> UpdateAsync(int id, CreateRoleDTO dto)
        {
            var role =await roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);  // another way
            if (role == null) throw new NotFoundException(localizer["rolenotfound"].Value);

            role.Name = dto.Name;
            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException(localizer["roleupdatefailed"].Value + ": " + errors);
            }
            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
            };
        }
    }
}
