using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Review
{
    public class CreateReviewDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public int CustomerId { get; set; }
        public int MealId { get; set; }
    }
}
