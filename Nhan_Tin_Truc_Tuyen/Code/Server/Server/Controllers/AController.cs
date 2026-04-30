using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")] // api/a
    [ApiController]
    public class AController : ControllerBase
    {
        [HttpGet] // /api/a
        public IActionResult Get()
        {
            return Ok("Xin chào người dùng!");
        }
    }
}
