using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.OrderItem
{
    public class CreateOrderItemDTO
    {
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        [JsonIgnore]
        public int TotalPrice => Quantity * UnitPrice;
        public int MealId { get; set; }
        public int OrderId { get; set; }
    }
}
