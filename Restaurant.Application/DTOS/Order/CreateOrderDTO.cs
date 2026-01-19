using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOS.Order
{
    public class CreateOrderDTO
    {
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [JsonIgnore]
        public int TotalPrice { get; set; }
        [JsonIgnore]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public int CustomerId { get; set; }
    }
}
