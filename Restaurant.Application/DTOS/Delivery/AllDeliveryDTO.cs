using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOS.Delivery
{
    public class AllDeliveryDTO
    {
        public int DeliveryId { get; set; }
        public string Address { get; set; }
        public DateTime DeliveryTime { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DeliveryStatus Status { get; set; }
    }
}
