using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Cart;
using Restaurant.Application.DTOS.CartItem;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.CartSR
{
    public interface ICartServices
    {
        Task<List<AllCartDTO>> GetAllCartsAsync();
        Task<AllCartDTO> GetCartByIdAsync(int id);
        Task<AllCartItemDTO> CreateCartAsync(int userid,int mealid ,int quantity);
        Task<AllCartItemDTO> UpdateCartAsync(int userid, int mealid, int quantity);
        Task<AllCartDTO> GetCartSummaryAsync(int userid);
        Task<bool> RemoveMealAsync(int mealid,int userid);
        Task<bool> ClereCartAsync(int userid);
    }
}
