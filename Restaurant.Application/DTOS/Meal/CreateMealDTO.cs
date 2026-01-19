using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Meal
{
    public class CreateMealDTO
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        [JsonIgnore]
        public int ChefId { get; set; }
    }
}
