using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Models
{
    public class Meal
    {
        public int MealId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; } 
        public int ChefId { get; set; }
        [ForeignKey("ChefId")]
        public Chef Chef { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
