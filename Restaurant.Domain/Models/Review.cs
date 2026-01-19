using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public int MealId { get; set; }
        [ForeignKey("MealId")]
        public Meal Meal { get; set; }
    }
}
