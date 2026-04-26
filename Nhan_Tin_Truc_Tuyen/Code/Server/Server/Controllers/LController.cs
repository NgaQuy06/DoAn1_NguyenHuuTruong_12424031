using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.X86;

namespace Server.Controllers
{
    [Route("api/[controller]")] // api/l
    [ApiController]
    public class LController : ControllerBase
    {
        [HttpGet] // /api/l
        public IActionResult Get()
        {
            return Ok("Xin chào người dùng");
        }

        string str = "Host=aws-0-ap-southeast-1.pooler.supabase.com;" +
                     "Port=6543;" +
                     "Database=postgres;" +
                     "Username=postgres;" +
                     "Password=Nguyentrg2006$;" +
                     "SSL Mode=Require;" +
                     "Trust Server Certificate=true;";

        [HttpPost("dangnhap")] // /api/l/dangnhap
        public IActionResult DangNhap([FromBody] LoginRequest req)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT * FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u AND \"MatKhau\" = @p AND \"QuyenHan\" = @r";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", req.Username.Trim());
                    cmd.Parameters.AddWithValue("p", req.Password.Trim());
                    cmd.Parameters.AddWithValue("r", req.Role.Trim());
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return Ok(new { message = "Đăng nhập thành công" });
                    }
                    else
                    {
                        return BadRequest(new { message = "Sai tài khoản hoặc mật khẩu" });
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("DangKy")]
        public IActionResult DangKy([FromBody] RegisterRequest req)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "INSERT INTO public.\"TaiKhoan\" (\"TenTK\", \"MatKhau\", \"Email\", \"TrangThai\", \"BietDanh\", \"NgayTao\") VALUES (@a, @b, @c, @d, @e, @f)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("a", req.Username.Trim());
                    cmd.Parameters.AddWithValue("b", req.Password.Trim());
                    if (string.IsNullOrWhiteSpace(req.Email))
                        cmd.Parameters.AddWithValue("c", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("c", req.Email.Trim());
                    cmd.Parameters.AddWithValue("d", "Không hoạt động");
                    cmd.Parameters.AddWithValue("e", "Người dùng mới");
                    cmd.Parameters.AddWithValue("f", DateTime.Now);
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
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
