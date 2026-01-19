using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Authorize;
using Restaurant.Infrastructure.Services.Authorize;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IAuthorizeService authorizeService;
        public RoleController(IAuthorizeService authorizeService)
        {
            this.authorizeService = authorizeService;
        }
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await authorizeService.GetAllAsync();
            return Ok(roles);
        }
        [HttpGet("roleid")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRoleById([FromQuery] int id)
        {
            var role = await authorizeService.GetByIdAsync(id);
            return Ok(role);
        }
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO dto)
        {
            var role = await authorizeService.CreateAsync(dto);
            return Ok(role);
        }
        [HttpPut("roleid")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole([FromQuery] int id, [FromBody] CreateRoleDTO dto)
        {
            var role = await authorizeService.UpdateAsync(id, dto);
            return Ok(role);
        }
        [HttpDelete("roleid")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole([FromQuery] int id)
        {
            var result = await authorizeService.DeleteAsync(id);
            if(!result) return BadRequest("Deleted Failed");
            return Ok("Deleted Successfully");
        }
    } 
}
