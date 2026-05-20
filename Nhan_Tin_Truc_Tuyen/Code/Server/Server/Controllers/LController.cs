using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
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

        [HttpPost("quenmatkhau")]
        public async Task<IActionResult> QuenMatKhau([FromBody] ForgotPasswordRequest req)
        {
            try
            {
                bool tenTK = await Npg.KiemTraTenTK(req.tenTK);
                if (!tenTK)
                {
                    return BadRequest(new { message = "Tên tài khoản không tồn tại!" });
                }
                bool email = await Npg.KiemTraEmail(req.tenTK, req.email);
                if (!email)
                {
                    return BadRequest(new { message = "Email không tồn tại!" });
                }
                else
                {
                    Random rd = new Random();
                    int otp = rd.Next(100000, 999999);
                    ChatHub.dsOTP[req.email] = otp;
                    await GuiMail(req.email, otp);
                    return Ok(new { message = "Ok" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        public static async Task GuiMail(string toEmail, int otp)
        {
            var from = "cubietgay@gmail.com";
            var password = "nhif fvxa mwjs worf";
            using var message = new MailMessage();

            message.From = new MailAddress(from);
            message.Subject = "Mã xác thực quên mật khẩu";
            message.Body = "Mã OTP của bạn là: " + otp + "\n Mã này sẽ hết hạn sau 5 phút.";

            message.To.Add(toEmail);
            using var smtp = new SmtpClient("smtp.gmail.com", 587);

            smtp.Credentials = new NetworkCredential(from, password);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(message);
        }

        [HttpPost("maxacthuc")]
        public async Task<IActionResult> MaXacThuc([FromBody] OtpRequest req)
        {
            try
            {
                if (ChatHub.dsOTP.ContainsKey(req.email))
                {
                    if (ChatHub.dsOTP[req.email] == req.otp)
                    {
                        ChatHub.dsOTP.Remove(req.email);
                        return Ok(new { message = "OTP đúng!" });
                    }
                }
                return BadRequest(new { message = "OTP sai!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
