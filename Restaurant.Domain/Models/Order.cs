using System.ComponentModel.DataAnnotations.Schema;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public Delivery? Delivery { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Payment Payment { get; set; }
    }
}
