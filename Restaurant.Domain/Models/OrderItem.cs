using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice => Quantity * UnitPrice;
        public int MealId { get; set; }
        [ForeignKey("MealId")]
        public Meal Meal { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
