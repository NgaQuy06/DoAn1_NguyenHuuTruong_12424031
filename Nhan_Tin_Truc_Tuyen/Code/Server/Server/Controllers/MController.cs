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

        [HttpGet("tinnhandiendan")] // /api/m/tinnhandiendan
        public async Task<IActionResult> TinNhanDienDan()
        {
            try
            {
                var list = await Npg.TinNhanDienDan();
                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("tinnhanrieng")] // /api/m/tinnhanrieng
        public async Task<IActionResult> TinNhanRieng([FromQuery] string user)
        {
            try
            {
                var list = await Npg.TinNhanRieng(user);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
