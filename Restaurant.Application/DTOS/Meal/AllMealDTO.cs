using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Meal
{
    public class AllMealDTO
    {
        public int MealId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int ChefId { get; set; }
    }
}
