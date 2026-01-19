using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Cart;
using Restaurant.Application.DTOS.Order;
using Restaurant.Application.DTOS.OrderItem;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services.OrderSR
{
    public interface IOrderServices
    {
        Task<List<AllOrderDTO>> GetAllOrdersAsync();
        Task<AllOrderDTO> GetOrderByIdAsync(int orderId);
        Task<AllOrderDTO> CheckOutAsync(CreateOrderDTO dto, int userid);
        Task<AllCartDTO> ReorderAsync(int orderId, int userid);
        Task<AllOrderDTO> CancleOrderAsync(int orderId, int userid);
        Task<OrderStatusDTO> GetOrderStatusAsync(int orderId);
        Task<AllOrderDTO>UpdateOrderStatusAsync(int orderId, OrderStatus status, int userid);
        Task<List<AllOrderItemDTO>> GetOrderItemsAsync(int orderId);
        Task<bool> DeleteOrderAsynce(int orderId,int userid);
    }
}
