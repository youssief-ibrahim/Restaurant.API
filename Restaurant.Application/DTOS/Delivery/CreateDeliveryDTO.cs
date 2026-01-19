using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOS.Delivery
{
    public class CreateDeliveryDTO
    {
        public string Address { get; set; }
        public DateTime DeliveryTime { get; set; } = DateTime.Now;
        public int OrderId { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
       public DeliveryStatus Status { get; set; }= DeliveryStatus.Pending;
    }
}
