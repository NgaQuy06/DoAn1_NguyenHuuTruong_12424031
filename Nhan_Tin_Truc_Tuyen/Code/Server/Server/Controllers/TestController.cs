using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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

        string str = "Host=db.fauxrzhhtdiesxfxuftz.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=nguyentrg2006;SSL Mode=Require;Trust Server Certificate=true;";

        [HttpPost("login")] // /api/test/login
        public IActionResult Login([FromBody] LoginRequest req)
        {
            try
            {
                using (var conn = new NpgsqlConnection(str))
                {
                    conn.Open();
                    Console.WriteLine("Kết nối thành công!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi: " + e.Message);
            }
            if (req.Username == "admin" && req.Password == "1234")
            {
                return Ok(new { message = "Đăng nhập thành công" });
            }
            return BadRequest(new { message = "Sai tài khoản" });
        }
    }
}
