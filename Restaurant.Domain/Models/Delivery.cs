using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Models
{
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public string Address { get; set; }
        public DateTime DeliveryTime { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int UserId { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    }
}
