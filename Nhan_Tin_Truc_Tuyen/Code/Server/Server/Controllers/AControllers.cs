using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")] // api/a
    [ApiController]
    public class AControllers : ControllerBase
    {
        [HttpGet] // /api/a
        public IActionResult Get()
        {
            return Ok("Xin chào người dùng");
        }

        [HttpPost("Account")] // /api/a/Account
        public IActionResult Account([FromBody] AccountRequest req)
        {

        }
    }
}
