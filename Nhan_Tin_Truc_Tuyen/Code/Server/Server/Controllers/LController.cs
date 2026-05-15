using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

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
        public async Task<IActionResult> DangNhap([FromBody] LoginRequest req)
        {
            try
            {
                string siu = await Npg.KiemTraTrangThai(req.Username);
                if (siu == "Đang trực tuyến")
                    return BadRequest("Tài khoản đang trực tuyến!");
                else if (siu == "Đã bị khóa")
                    return BadRequest("Tài khoản bị khóa!");
                var result = await Npg.DangNhap(req.Username, req.Password, req.Role);
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
        public async Task<IActionResult> DangKy([FromBody] RegisterRequest req)
        {
            bool ok = await Npg.XacMinhTaiKhoan(req.Captcha);

            if (!ok)
            {
                return BadRequest(new
                {
                    message = "CAPTCHA không hợp lệ!"
                });
            }
            try
            {
                if (await Npg.KiemTraTenTK(req.TenTK))
                {
                    return BadRequest(new { message = "Tên tài khoản đã tồn tại!" });
                }

                string result = await Npg.DangKy(req.TenTK, req.MatKhau, req.Email, req.Sdt);
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
