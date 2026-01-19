using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Review
{
    public class AllReviewDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }

        public int CustomerId { get; set; }
        public int MealId { get; set; }
    }
}
