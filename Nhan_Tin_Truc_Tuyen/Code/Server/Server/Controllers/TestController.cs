using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Reflection.PortableExecutable;
using DotNetEnv;

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

        string str = "Host=aws-1-ap-northeast-1.pooler.supabase.com;" +
                     "Port=6543;" +
                     "Database=postgres;" +
                     "Username=postgres.fauxrzhhtdiesxfxuftz;" +
                     "Password=Nguyentrg2006$;" +
                     "SSL Mode=Require;" +
                     "Trust Server Certificate=true;";

        [HttpPost("login")] // /api/test/login
        public IActionResult Login([FromBody] LoginRequest req)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT * FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u AND \"MatKhau\" = @p";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", req.Username.Trim());
                    cmd.Parameters.AddWithValue("p", req.Password.Trim());
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return Ok(new { message = "Đăng nhập thành công" });
                    }
                    else
                    {
                        return BadRequest(new { message = "Sai tài khoản" });
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
