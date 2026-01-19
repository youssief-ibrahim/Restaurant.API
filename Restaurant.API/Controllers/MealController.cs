using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Meal;
using Restaurant.Application.Services.MealSR;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly IMealServices mealServices;
        public MealController(IMealServices mealServices)
        {
            this.mealServices = mealServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var res = await mealServices.GetAllMealAsync();
            return Ok(res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyidAsync(int id)
        {
            var res = await mealServices.GetMealByIdAsync(id);
            return Ok(res);
        }
        [HttpGet("by-nmae/{name}")]
        public async Task<IActionResult> GetMealByNameAsync(string name)
        {
            var res=await mealServices.GetMealByNameAsync(name);
            return Ok(res);
        }
        [HttpGet("by-rating/{rate}")]
        public async Task<IActionResult> GetMealByRateAsync(int rate)
        {
            var res = await mealServices.GetMealByRateAsync(rate);
            return Ok(res);
        }
        [HttpPost]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMealDTO dto)
        {
            var userid=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await mealServices.CreateMealAsync(dto, userid);
            return Ok(res);
        }
        [HttpPut("{mealid}")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> UpdateAsync([FromBody] CreateMealDTO dto, int mealid)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await mealServices.UpdateMealAsync(dto, userid, mealid);
            return Ok(res);
        }
        [HttpDelete("{mealid}")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> DeleteAsync(int mealid)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await mealServices.DeleteMealAsync(userid, mealid);
            if (!res)
                return BadRequest("Delete Failed");
            return Ok("Deleted Sussfully");
        }
    }
}
