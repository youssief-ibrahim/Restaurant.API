using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Review;

namespace Restaurant.Application.Services.CustomerSR
{
    public interface ICustomerServices
    {
        Task<List<AllCustomerDTO>> GetAllAsync();
        Task<AllCustomerDTO> GetbyidAsync(int id);
        Task<List<AllReviewDTO>> GetAllCustomerReviewAsync(int userid);
        Task<AllCustomerDTO> CreateAsync(CreateCustomerDTO dto,int userid);
        Task<AllCustomerDTO> UpdateAsync(CreateCustomerDTO dto, int userid);
        Task<bool> DeleteAsync(int userid);

    }
}
