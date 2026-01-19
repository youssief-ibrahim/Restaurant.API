using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public decimal Amount { get; set; }

        public string Provider { get; set; } = "Stripe";

        public string? StripePaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
