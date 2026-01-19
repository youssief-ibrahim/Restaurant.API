using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.Services.CustomerSR;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices customerServices;
        public CustomerController(ICustomerServices customerServices)
        {
            this.customerServices = customerServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var res = await customerServices.GetAllAsync();
            return Ok(res);
        }
        [HttpGet("Customer-Reviews")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllCustomerReviewAsync()
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await customerServices.GetAllCustomerReviewAsync(userid);
            return Ok(res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyidAsync(int id)
        {
            var res = await customerServices.GetbyidAsync(id);
            return Ok(res);
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerDTO dto)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var res = await customerServices.CreateAsync(dto,userid);
            return Ok(res);
        }
        [HttpPut]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateAsync([FromBody] CreateCustomerDTO dto)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await customerServices.UpdateAsync(dto, userid);
            return Ok(res);
        }
        [HttpDelete]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteAsync()
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await customerServices.DeleteAsync(userid);
            if(!res)
                return BadRequest("Delete Failed");
            return Ok("Deleted Sussfully");
        }

    }
}
