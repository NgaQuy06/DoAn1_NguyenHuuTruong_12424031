using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Reflection.PortableExecutable;

namespace Server.Controllers
{
    [Route("api/[controller]")] // controller = test
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet] // /api/test
        public IActionResult Get()
        {
            return Ok("Hello từ API");
        }

        string str = "postgresql://postgres:Nguyentrg2006$@db.fauxrzhhtdiesxfxuftz.supabase.co:5432/postgres";

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
