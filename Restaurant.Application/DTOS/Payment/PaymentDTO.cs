using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.DTOS.Payment
{
    public class PaymentDTO
    {
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
