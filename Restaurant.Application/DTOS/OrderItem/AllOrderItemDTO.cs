using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.OrderItem
{
    public class AllOrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
        public int MealId { get; set; }
        public int OrderId { get; set; }
    }
}
