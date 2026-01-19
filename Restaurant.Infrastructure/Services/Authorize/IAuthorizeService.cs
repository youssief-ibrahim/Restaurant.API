using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Authorize;

namespace Restaurant.Infrastructure.Services.Authorize
{
    public interface IAuthorizeService
    {
        Task<List<RoleDTO>> GetAllAsync();
        Task<RoleDTO> GetByIdAsync(int id);
        Task<RoleDTO> CreateAsync(CreateRoleDTO dto);
        Task<RoleDTO> UpdateAsync(int id, CreateRoleDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
