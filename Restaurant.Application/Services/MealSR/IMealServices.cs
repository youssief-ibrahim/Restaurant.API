using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Meal;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.MealSR
{
    public interface IMealServices
    {
        Task<List<AllMealDTO>> GetAllMealAsync();
        Task<AllMealDTO> GetMealByIdAsync(int mealId);
        Task<AllMealDTO> GetMealByNameAsync(string name);
        Task<List<AllMealDTO>> GetMealByRateAsync(int rate);
        Task<AllMealDTO> CreateMealAsync(CreateMealDTO dto, int userid);
        Task<AllMealDTO> UpdateMealAsync(CreateMealDTO dto, int userid,int mealid);
        Task<bool> DeleteMealAsync(int userid,int mealid);

    }
}
