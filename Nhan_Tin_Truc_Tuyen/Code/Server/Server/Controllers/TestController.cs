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
                if (req.Type.Trim() == "dangnhap")
                {
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
                else if (req.Type.Trim() == "dangky")
                {
                    string sql = "INSERT INTO public.\"TaiKhoan\" (\"TenTK\", \"MatKhau\", \"Email\", \"NgayTao\") VALUES (@a, @b, @c, @d)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("a", req.Username.Trim());
                        cmd.Parameters.AddWithValue("b", req.Password.Trim());
                        cmd.Parameters.AddWithValue("c", req.Email.Trim());
                        cmd.Parameters.AddWithValue("d", DateTime.Now);
                        int reader = cmd.ExecuteNonQuery();
                        if (reader > 0)
                        {
                            return Ok(new { message = "Đăng ký thành công" });
                        }
                        else
                        {
                            return BadRequest(new { message = "Đăng ký thất bại" });
                        }
                    }
                }
                else
                {
                    return BadRequest(new { message = "Yêu cầu không hợp lệ" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
