using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOS.Order
{
    public class AllOrderDTO
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; } 
        public int TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } 
        public int CustomerId { get; set; }
    }
}
