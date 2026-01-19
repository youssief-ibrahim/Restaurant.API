using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }
        public int MealId { get; set; }
        [ForeignKey("MealId")]
        public Meal Meal { get; set; }
    }
}
