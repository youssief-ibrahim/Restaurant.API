using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Services.CartSR;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServices cartServices;
        public CartController(ICartServices cartServices)
        {
            this.cartServices = cartServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var res = await cartServices.GetAllCartsAsync();
            return Ok(res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            var res = await cartServices.GetCartByIdAsync(id);
            return Ok(res);
        }
        [HttpPost("{mealid}/{quantity}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateCart(int mealid, int quantity)
        {
            var userid=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await cartServices.CreateCartAsync(userid,mealid, quantity);
            return Ok(res);
        }
        [HttpPut("{mealid}/{quantity}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCart(int mealid, int quantity)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await cartServices.UpdateCartAsync(userid, mealid, quantity);
            return Ok(res);
        }
        [HttpGet("summary")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCartSummary()
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await cartServices.GetCartSummaryAsync(userid);
            return Ok(res);
        }
        [HttpDelete("remove/{mealid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveMeal(int mealid)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await cartServices.RemoveMealAsync(mealid, userid);
            if (!res)
                return BadRequest("Remove Meal Failed");
            return Ok("Meal Removed Successfully");
        }
        [HttpDelete("clear")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ClearCart()
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await cartServices.ClereCartAsync(userid);
            if(!res)
                return BadRequest("Clear Cart Failed");
            return Ok("Cart Cleared Successfully");
        }
    }
}
