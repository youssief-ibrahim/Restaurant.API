using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Chef;
using Restaurant.Application.Services.ChefSR;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChefController : ControllerBase
    {
        private readonly IChefServices chefServices;
        public ChefController(IChefServices chefServices)
        {
            this.chefServices = chefServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var res = await chefServices.GetAllAsync();
            return Ok(res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyidAsync(int id)
        {
            var res = await chefServices.GetbyidAsync(id);
            return Ok(res);
        }
        [HttpPost]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateChefDTO dto)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await chefServices.CreateAsync(dto, userid);
            return Ok(res);
        }
        [HttpPut]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> UpdateAsync([FromBody] CreateChefDTO dto)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await chefServices.UpdateAsync(dto, userid);
            return Ok(res);
        }
        [HttpDelete]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> DeleteAsync()
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await chefServices.DeleteAsync(userid);
            if (!res)
                return BadRequest("Delete Failed");
            return Ok("Deleted Sussfully");
        }
    }
}
