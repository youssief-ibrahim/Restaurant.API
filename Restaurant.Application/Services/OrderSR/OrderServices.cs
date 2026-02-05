using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Restaurant.Application.DTOS.Cart;
using Restaurant.Application.DTOS.Order;
using Restaurant.Application.DTOS.OrderItem;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.OrderSR
{
    public class OrderServices : IOrderServices
    {
        private readonly IGenericRebosatory<Order> GenOrder;
        private readonly IGenericRebosatory<OrderItem> GenOrderItem;
        private readonly IGenericRebosatory<CartItem> GenCartItem;
        private readonly IGenericRebosatory<Customer> GenCustomer;
        private readonly IGenericRebosatory<Meal> GenMeal;
        private readonly IGenericRebosatory<Cart> GenCart;
        private readonly IGenericRebosatory<Payment> GenPayment;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<OrderServices> localizer;

        public OrderServices(IGenericRebosatory<Order> GenOrder, IGenericRebosatory<OrderItem> GenOrderItem, IGenericRebosatory<Customer> GenCustomer, IGenericRebosatory<Meal> GenMeal, IGenericRebosatory<Cart> GenCart, IGenericRebosatory<CartItem> GenCartItem, IGenericRebosatory<Payment> GenPayment, IUnitOfWork unitOfWork,IMapper mapper, IStringLocalizer<OrderServices> localizer)
        {
            this.GenOrder = GenOrder;
            this.GenOrderItem = GenOrderItem;
            this.GenCustomer = GenCustomer;
            this.GenMeal = GenMeal;
            this.GenCart = GenCart;
            this.GenCartItem = GenCartItem;
            this.GenPayment = GenPayment;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<List<AllOrderDTO>> GetAllOrdersAsync()
        {
            var order = await GenOrder.GetAll();
            if (order == null || !order.Any())
                throw new NotFoundException("No Orders Found");
            var orderDto = mapper.Map<List<AllOrderDTO>>(order);
            return orderDto;
        }

        public async Task<AllOrderDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await GenOrder.GetById(s => s.OrderId == orderId);
            if (order == null)
                throw new NotFoundException($"No Found Order with  id {orderId}");
            var orderDto = mapper.Map<AllOrderDTO>(order);
            return orderDto;
        }

        public  async Task<List<AllOrderItemDTO>> GetOrderItemsAsync(int orderId)
        {
            var res = await GenOrderItem.FindAll(s => s.OrderId == orderId);
            if (!res.Any())
                throw new NotFoundException($"No Found Order Items with  order id {orderId}");
            var orderDto = mapper.Map<List<AllOrderItemDTO>>(res);
            return orderDto;
        }

        public async Task<OrderStatusDTO> GetOrderStatusAsync(int orderId)
        {
            var order = await GenOrder.GetById(s => s.OrderId == orderId);
            if (order == null)
                throw new NotFoundException($"No Found Order with  id {orderId}");
            var data = mapper.Map<OrderStatusDTO>(order);
            return data;
        }

        public async Task<AllOrderDTO> CheckOutAsync(CreateOrderDTO dto, int userid)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await GenCustomer.GetById(s => s.UserId == userid);
                if (customer == null)
                    throw new NotFoundException($"No Found Customer with  userid {userid}");
                var cart = await GenCart.GetById(s => s.CustomerId == customer.CustomerId);
                if (cart == null)
                    throw new NotFoundException($"No Found Cart for Customer with  userid {userid}");

                var cartItems = await GenCartItem.FindAll(s => s.CartId == cart.CartId, r => r.Meal);
                if (!cartItems.Any())
                    throw new NotFoundException($"No Found CartItems in Cart for Customer with  userid {userid}");

                foreach (var item in cartItems)
                {
                    if (item.Meal.Quantity < item.Quantity)
                        throw new BadRequestException($"Meal {item.Meal.Name} does not have enough quantity.");
                }

                var order = mapper.Map<Order>(dto);
                order.CustomerId = customer.CustomerId;
                order.TotalPrice = cartItems.Sum(i => i.Quantity * i.Meal.Price);
                await GenOrder.Create(order);
                await unitOfWork.SaveChanges();
               

                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        MealId = item.MealId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Meal.Price
                    };

                    await GenOrderItem.Create(orderItem);
                }
                await unitOfWork.SaveChanges();
                var payment=new Payment
                {
                    OrderId = order.OrderId,
                    Amount = order.TotalPrice,
                    Provider = "Stripe",
                    Status = PaymentStatus.Pending
                };
                await GenPayment.Create(payment);
                await unitOfWork.SaveChanges();
                cart.OrderId = order.OrderId;
                GenCart.update(cart);
                await unitOfWork.SaveChanges();
                await unitOfWork.CommitAsync();

                var orderDto = mapper.Map<AllOrderDTO>(order);
                return orderDto;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                var msg = ex.InnerException?.Message;
                throw new Exception(msg, ex);
            }

        }

        public async Task<AllCartDTO> ReorderAsync(int orderId, int userid)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await GenCustomer.GetById(s => s.UserId == userid);
                if (customer == null)
                    throw new NotFoundException($"No Found Customer with  userid {userid}");

                var cart = await GenCart.GetQueryable(c => c.CartItems)
                    .Where(c => c.CustomerId == customer.CustomerId)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Meal)
                    .FirstOrDefaultAsync();

                if (cart == null)
                    throw new NotFoundException($"No Found Cart for Customer with  userid {userid}");

                var previousorder = await GenOrder.GetById(s => s.OrderId == orderId && s.CustomerId == customer.CustomerId);
                if (previousorder == null)
                    throw new NotFoundException($"No Found Order with  id {orderId} for Customer with  userid {userid}");

                var previousOrderItems = await GenOrderItem.FindAll(s => s.OrderId == orderId, r => r.Meal);
                if (!previousOrderItems.Any())
                    throw new NotFoundException($"Order has no items");

                foreach (var item in previousOrderItems)
                {

                    var cartItem = cart.CartItems.FirstOrDefault(s => s.MealId == item.MealId);

                    var eisxtQuantity = cartItem?.Quantity ?? 0;
                    int totalquantity = eisxtQuantity + item.Quantity;

                    if (totalquantity > item.Meal.Quantity)
                    {
                        int availableQuantity = item.Meal.Quantity - eisxtQuantity;
                        if (availableQuantity > 0) throw new BadRequestException($"You can only add {availableQuantity} {item.Meal.Name}");
                        else throw new BadRequestException("Out Of Stouk");
                    }

                    if (cartItem != null)
                    {
                        cartItem.Quantity += item.Quantity;
                        GenCartItem.update(cartItem);
                    }
                    else
                    {
                        var newCartItem = new CartItem
                        {
                            CartId = cart.CartId,
                            MealId = item.MealId,
                            Quantity = item.Quantity
                        };
                        cart.CartItems.Add(newCartItem);
                    }
                }
                await unitOfWork.SaveChanges();
                await unitOfWork.CommitAsync();
                var cartDto = mapper.Map<AllCartDTO>(cart);
                return cartDto;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                throw;
            }

        }

        public async Task<AllOrderDTO> UpdateOrderStatusAsync(int orderId, OrderStatus status, int userid)
        {
            var customer = await GenCustomer.GetById(s => s.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"No Found Customer with  userid {userid}");
            var order = await GenOrder.GetById(s => s.OrderId == orderId && s.CustomerId == customer.CustomerId);
            if (order == null)
                throw new NotFoundException($"No Found Order with  id {orderId}");
            if (status != OrderStatus.Cancelled)
                throw new BadRequestException("You can only cancel the order");
            order.Status = status;
            GenOrder.update(order);
            await unitOfWork.SaveChanges();
            var orderDto = mapper.Map<AllOrderDTO>(order);
            return orderDto;
        }
        public async Task<AllOrderDTO> CancleOrderAsync(int orderId, int userid)
        {
            var customer = await GenCustomer.GetById(s => s.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"No Found Customer with  userid {userid}");
            var order = await GenOrder.GetById(s => s.OrderId == orderId && s.CustomerId == customer.CustomerId);
            if (order == null)
                throw new NotFoundException($"No Found Order with  id {orderId}");
            if (order.Status == OrderStatus.Cancelled)
                throw new BadRequestException($"Order with id {orderId} is already canceled.");
            order.Status = OrderStatus.Cancelled;
            GenOrder.update(order);
            await unitOfWork.SaveChanges();
            var orderDto = mapper.Map<AllOrderDTO>(order);
            return orderDto;
        }
        public async Task<bool> DeleteOrderAsynce(int orderId, int userid)
        {
            var customer = await GenCustomer.GetById(s => s.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"No Found Customer with  userid {userid}");
            var order = await GenOrder.GetById(s => s.OrderId == orderId && s.CustomerId == customer.CustomerId);
            if (order == null)
                throw new NotFoundException($"No Found Order with  id {orderId}");
            GenOrder.delete(order);
            await unitOfWork.SaveChanges();
            return true;
        }
      
    }
}
