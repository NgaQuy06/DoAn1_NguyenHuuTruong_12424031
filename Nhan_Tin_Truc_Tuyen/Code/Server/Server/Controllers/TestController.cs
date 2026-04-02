using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")] // controller = test
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet] // /api/test
        public IActionResult Get()
        {
            return Ok("Hello từ API 🚀");
        }

        [HttpPost("login")] // /api/test/login
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req.Username == "admin" && req.Password == "1234")
            {
                return Ok(new { message = "Đăng nhập thành công" });
            }
            return BadRequest(new { message = "Sai tài khoản" });
        }
    }
}
