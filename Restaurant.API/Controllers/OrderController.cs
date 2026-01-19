using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Order;
using Restaurant.Application.Services.OrderSR;
using Restaurant.Domain.Enums;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices orderServices;
        public OrderController(IOrderServices orderServices)
        {
            this.orderServices = orderServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var res =await orderServices.GetAllOrdersAsync();
            return Ok(res);
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var res = await orderServices.GetOrderByIdAsync(orderId);
            return Ok(res);
        }
        [HttpGet("order-status/{orderid}")]
        public async Task<IActionResult> GetOrderStatus(int orderid)
        {
            var res = await orderServices.GetOrderStatusAsync(orderid);
            return Ok(res);
        }
        [HttpPut("update-order-status/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] OrderStatus status)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await orderServices.UpdateOrderStatusAsync(orderId, status, userid);
            return Ok(res);
        }
        [HttpPut("Cancel-order/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await orderServices.CancleOrderAsync(orderId, userid);
            return Ok(res);
        }
        [HttpGet("order-items/{orderId}")]
        public async Task<IActionResult> GetOrderItems(int orderId)
        {
            var res = await orderServices.GetOrderItemsAsync(orderId);
            return Ok(res);
        }

        [HttpPost("checkout")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CheckOut([FromBody] CreateOrderDTO dto)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await orderServices.CheckOutAsync(dto, userid);
            return Ok(res);
        }
        [HttpPost("Reorder/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Reorder(int orderId)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await orderServices.ReorderAsync(orderId, userid);
            return Ok(res);
        }
        [HttpDelete("{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await orderServices.DeleteOrderAsynce(orderId, userid);
            if(!res) return BadRequest("Fail to delete order.");
            else return Ok("Order deleted successfully.");
        }
    }
}
