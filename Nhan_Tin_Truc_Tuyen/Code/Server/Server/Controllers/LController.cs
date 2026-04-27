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

        [HttpPost("dangnhap")] // /api/l/dangnhap
        public IActionResult DangNhap([FromBody] LoginRequest req)
        {
            try
            {
                string result = Npg.DangNhap(req.Username, req.Password, req.Role);
                if (result == "Ok")
                {
                    return Ok(new { message = "Đăng nhập thành công!" });
                }
                else
                {
                    return BadRequest(new { message = result });
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
            try
            {
                string result = Npg.DangKy(req.Username, req.Password, req.Email);
                if (result == "Ok")
                {
                    return Ok(new { message = "Đăng ký thành công!" });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
