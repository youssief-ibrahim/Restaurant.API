using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Cart;
using Restaurant.Application.DTOS.Payment;

namespace Restaurant.Application.Services.PaymentSR
{
    public interface IPaymentService
    {
        //Task<AllCartDTO> CreateOrUpdatePaymentIntent(int cartId);
        Task<PaymentDTO> CreateOrUpdatePaymentIntentAsync(int cartId);
        Task<bool> ConfirmPaymentAsync(string paymentIntentId);
        Task<bool> FailPaymentAsync(string paymentIntentId);

    }
}
