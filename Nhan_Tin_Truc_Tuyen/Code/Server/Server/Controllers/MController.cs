using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")] // api/m
    [ApiController]
    public class MController : ControllerBase
    {
        [HttpGet] // /api/m
        public IActionResult Get()
        {
            return Ok("Xin chào người dùng!");
        }

        [HttpPost("tinnhandiendan")] // /api/m/tinnhandiendan
        public IActionResult TinNhanDienDan()
        {
            try
            {
                var list = Npg.TinNhanDienDan();
                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
