using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Delivery;
using Restaurant.Application.Services.DeliverySR;
using Restaurant.Domain.Enums;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryServices deliveryServices;
        public DeliveryController(IDeliveryServices deliveryServices)
        {
            this.deliveryServices = deliveryServices;
        }
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDeliveries()
        {
            var res = await deliveryServices.GetAllAsync();
            return Ok(res);
        }
        [HttpGet("{deliveryid}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeliveryById(int deliveryid)
        {
            var res = await deliveryServices.GetByIdAsync(deliveryid);
            return Ok(res);
        }
        [HttpGet("order-id/{orderid}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeliveryByOrderId(int orderid)
        {
            var res = await deliveryServices.GetByOrderidAsync(orderid);
            return Ok(res);
        }
        [HttpPost("assign/{orderid}")]
        [Authorize(Roles = "Delivery")]
        public async Task<IActionResult> CreateDelivery(int orderid,string address)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await deliveryServices.CreateAsync(userid, orderid,address);
            return Ok(res);
        }
        [HttpPut("status/{orderid}")]
        [Authorize(Roles = "Delivery")]
        public async Task<IActionResult> UpdateDelivery( int orderid, DeliveryStatus status)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await deliveryServices.UpdateAsync(userid, orderid,status);
            return Ok(res);
        }
        [HttpDelete("cancel/{orderid}")]
        [Authorize(Roles = "Delivery")]
        public async Task<IActionResult> CancelDelivery(int orderid)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await deliveryServices.CancleAsync(userid, orderid);
           if(!res)
                return BadRequest("Unable to cancel the delivery.");
            return Ok("Delivery cancelled successfully.");
        }
    }
}
