using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Restaurant.Application.DTOS.Meal;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.MealSR
{
    public class MealServices : IMealServices
    {
        private readonly IGenericRebosatory<Meal> GenMeal;
        private readonly IGenericRebosatory<Chef> GenChef;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public MealServices(IGenericRebosatory<Meal> GenMeal, IUnitOfWork unitOfWork, IMapper mapper, IGenericRebosatory<Chef> GenChef)
        {
            this.GenMeal = GenMeal;
            this.GenChef = GenChef;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<List<AllMealDTO>> GetAllMealAsync()
        {
            var res = await GenMeal.GetAll();
            if (!res.Any())
                throw new NotFoundException("No Meals Found");
            var data = mapper.Map<List<AllMealDTO>>(res);
            return data;
        }
        public async Task<AllMealDTO> GetMealByIdAsync(int mealId)
        {
            var res = await GenMeal.GetById(g => g.MealId == mealId);
            if (res == null)
                throw new NotFoundException($"Not Found Meal with this Id {mealId}");

            var data = mapper.Map<AllMealDTO>(res);
            return data;
        }
        public async Task<AllMealDTO> GetMealByNameAsync(string name)
        {
            name = name.ToLower().Trim();
            var res = await GenMeal.GetById(g => g.Name.ToLower().Trim() == name);

            if (res == null)
                throw new NotFoundException($"Not Found Meal with this Name {name}");

            var data = mapper.Map<AllMealDTO>(res);
            return data;
        }
        public async Task<List<AllMealDTO>> GetMealByRateAsync(int rate)
        {
            var res = await GenMeal.FindAll(g => g.Reviews.Any(r => r.Rating == rate), m => m.Reviews);
            if (!res.Any())
                throw new NotFoundException($"No Meals Found with this Review {rate}");

            var data = mapper.Map<List<AllMealDTO>>(res);
            return data;
        }
        public async Task<AllMealDTO> CreateMealAsync(CreateMealDTO dto, int userid)
        {
            var chef = await GenChef.GetById(s => s.UserId == userid);
            if (chef == null)
                throw new NotFoundException($"No Found Chef with this UserId {userid}");

            var meal = mapper.Map<Meal>(dto);
            meal.ChefId = chef.ChefId;

            await GenMeal.Create(meal);
            await unitOfWork.SaveChanges();

            var data = mapper.Map<AllMealDTO>(meal);
            return data;
        }
        public async Task<AllMealDTO> UpdateMealAsync(CreateMealDTO dto, int userid, int mealid)
        {
            var chef = await GenChef.GetById(s => s.UserId == userid);
            if (chef == null)
                throw new NotFoundException($"No Found Chef with this UserId {userid}");
            var meal = await GenMeal.GetById(m => m.MealId == mealid);
            if(meal == null)
                throw new NotFoundException($"Not Found Meal with this Id {mealid}");
            if (chef.ChefId!=meal.ChefId)
                throw new NotFoundException("You are not allowed to update this meal");
            mapper.Map(dto, meal);
            meal.ChefId = chef.ChefId;
            GenMeal.update(meal);
            await unitOfWork.SaveChanges();
            var data = mapper.Map<AllMealDTO>(meal);
            return data;
        }
        public async Task<bool> DeleteMealAsync(int userid, int mealid)
        {
            var chef = await GenChef.GetById(s => s.UserId == userid);
            if (chef == null)
                throw new NotFoundException($"No Found Chef with this UserId {userid}");
            var meal = await GenMeal.GetById(m => m.MealId == mealid);
            if (meal == null)
                throw new NotFoundException($"Not Found Meal with this Id {mealid}");
            if (chef.ChefId != meal.ChefId)
                throw new NotFoundException("You are not allowed to Delete this meal");

            GenMeal.delete(meal);
            await unitOfWork.SaveChanges();
            return true;
        }

      

    



   

  
    }
}
