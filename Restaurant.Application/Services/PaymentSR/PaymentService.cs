using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Restaurant.Application.DTOS.Cart;
using Restaurant.Application.DTOS.Payment;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;
using Stripe;

namespace Restaurant.Application.Services.PaymentSR
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration config;
        private readonly IGenericRebosatory<Cart> Gencart;
        private readonly IGenericRebosatory<Order> GenOrder;
        private readonly IGenericRebosatory<Meal> GenMeal;
        private readonly IGenericRebosatory<Payment> GenPayment;
        private readonly IGenericRebosatory<OrderItem> GenOrderItem;
        private readonly IGenericRebosatory<CartItem> GenCartItem;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PaymentService(IConfiguration config,
            IGenericRebosatory<Cart> Gencart,
            IGenericRebosatory<Order> GenOrder,
            IGenericRebosatory<Payment> GenPayment,
            IGenericRebosatory<Meal> GenMeal,
            IGenericRebosatory<OrderItem> GenOrderItem,
            IGenericRebosatory<CartItem> GenCartItem,
        IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.config = config;
            this.Gencart = Gencart;
            this.GenOrder = GenOrder;
            this.GenPayment = GenPayment;
            this.GenMeal = GenMeal;
            this.GenOrderItem = GenOrderItem;
            this.GenCartItem = GenCartItem;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        //2)get cart by cartid=>ICartRepository
        //3)check price of cart items=>ICartRepository
        //4)get delivery method if any=>IDeliveryMethodRepository
        //5)calculate total Amount   
        //6)create or update payment intent.
        public async Task<PaymentDTO> CreateOrUpdatePaymentIntentAsync(int cartId)
        {
            StripeConfiguration.ApiKey = config["Stripe:Secretkey"];

            var cart = await Gencart.GetQueryable(c => c.CartItems)
                .Include(c => c.CartItems)
                .ThenInclude(c => c.Meal)
                .FirstOrDefaultAsync(c => c.CartId == cartId) ?? throw new NotFoundException("Cart not found");

            if (!cart.CartItems.Any())
                throw new BadRequestException("Cart is empty");

            foreach (var item in cart.CartItems)
            {
                var meal = await GenMeal.GetById(m => m.MealId == item.MealId);

                if (meal == null)
                    throw new NotFoundException($"Meal with id {item.MealId} not found");

                // 🔒 enforce DB price (important)
                if (item.Meal.Price != meal.Price)
                {
                    item.Meal.Price = meal.Price;
                }
            }
            var amount = (cart.CartItems.Sum(i => i.Quantity * i.Meal.Price) + cart.ShippingPrice) * 100;
            var stripeService = new PaymentIntentService();

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },

                };
                var intent = await stripeService.CreateAsync(options);
                cart.ClientSecret = intent.ClientSecret;
                cart.PaymentIntentId = intent.Id;

                var payment = await GenPayment.GetById(p =>
                    p.OrderId == cart.OrderId &&
                    p.Status == PaymentStatus.Pending);

                if (payment == null)
                    throw new NotFoundException("Payment not found for this cart");

                payment.StripePaymentIntentId = intent.Id;
                payment.ClientSecret = intent.ClientSecret;

                GenPayment.update(payment);
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)amount,
                };
                await stripeService.UpdateAsync(cart.PaymentIntentId, options);
            }
            Gencart.update(cart);
            await unitOfWork.SaveChanges();

            return new PaymentDTO
            {
                ClientSecret = cart.ClientSecret,
                PaymentIntentId = cart.PaymentIntentId,
                Status = PaymentStatus.Pending
            };
        }

        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId)
        {
            var payment = await GenPayment.GetById(
                p => p.StripePaymentIntentId == paymentIntentId);

            if (payment == null)
                return false;

            payment.Status = PaymentStatus.Paid;
            GenPayment.update(payment);

            var order = await GenOrder.GetById(o => o.OrderId == payment.OrderId);
            order.Status = OrderStatus.Confirmed;
            GenOrder.update(order);

          
            var orderItems = await GenOrderItem.FindAll(i => i.OrderId == order.OrderId, i => i.Meal);
            foreach (var item in orderItems)
            {
                item.Meal.Quantity -= item.Quantity;
                GenMeal.update(item.Meal);
            }

          
            //var cart = await Gencart.GetById(c => c.CustomerId == order.CustomerId); is also okkay
            var cart = await Gencart.GetById(c => c.OrderId == order.OrderId);
            var cartItems = await GenCartItem.FindAll(ci => ci.CartId == cart.CartId);
            foreach (var cartItem in cartItems)
            {
                GenCartItem.delete(cartItem);
            }

            await unitOfWork.SaveChanges();
            return true;
        }
        public async Task<bool> FailPaymentAsync(string paymentIntentId)
        {
            var payment = await GenPayment.GetById(
                p => p.StripePaymentIntentId == paymentIntentId);

            if (payment == null)
                return false;

            payment.Status = PaymentStatus.Failed;
            GenPayment.update(payment);

            var order = await GenOrder.GetById(o => o.OrderId == payment.OrderId);
            order.Status = OrderStatus.Cancelled;
            GenOrder.update(order);

            await unitOfWork.SaveChanges();
            return true;
        }

    }
}
