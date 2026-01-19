using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Chef;

namespace Restaurant.Application.Services.ChefSR
{
    public interface IChefServices
    {
        Task<List<AllChefDTO>> GetAllAsync();
        Task<AllChefDTO> GetbyidAsync(int id);
        Task<AllChefDTO> CreateAsync(CreateChefDTO dto, int userid);
        Task<AllChefDTO> UpdateAsync(CreateChefDTO dto, int userid);
        Task<bool> DeleteAsync(int userid);
    }
}
