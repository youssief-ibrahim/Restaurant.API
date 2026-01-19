using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Models
{
    public class Chef
    {
        public int ChefId { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public int UserId { get; set; }
        public ICollection<Meal> Meals { get; set; }= new List<Meal>();
    }
}
