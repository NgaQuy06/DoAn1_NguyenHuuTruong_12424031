using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")] // api/l
    [ApiController]
    public class LController : ControllerBase
    {
        [HttpGet] // /api/l
        public IActionResult Get()
        {
            return Ok("Xin chào người dùng!");
        }

        [HttpPost("dangnhap")] // /api/l/dangnhap
        public IActionResult DangNhap([FromBody] LoginRequest req)
        {
            try
            {
                var result = Npg.DangNhap(req.Username, req.Password, req.Role);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Đăng nhập thất bại!");
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
                    return Ok("Đăng ký thành công!");
                }
                else if (result == "Tên tài khoản đã tồn tại!")
                {
                    return BadRequest("Tên tài khoản đã tồn tại!");
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
