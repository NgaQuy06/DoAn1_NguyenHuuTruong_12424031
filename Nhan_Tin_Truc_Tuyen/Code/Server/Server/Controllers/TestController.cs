using Microsoft.AspNetCore.Http;
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

        string str = "postgres://postgres.fauxrzhhtdiesxfxuftz:Nguyentrg2006$@aws-0-ap-northeast-1.pooler.supabase.com:5432/postgres";

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
