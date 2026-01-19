using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]

        public int? OrderId { get; set; }
        // ===== Payment =====
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }

        // ===== Delivery =====
        public int? DeliveryMethodId { get; set; }
        public decimal ShippingPrice { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<CartItem> CartItems { get; set; }=new List<CartItem>();
    }
}
