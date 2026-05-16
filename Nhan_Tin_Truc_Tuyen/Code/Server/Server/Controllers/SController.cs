using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Server.Controllers
{
    [Route("api/[controller]")] // api/s
    [ApiController]
    public class SController : ControllerBase
    {
        [HttpGet] // /api/s
        public IActionResult Get()
        {
            return Ok("Bạn muốn tìm gì?");
        }

        [HttpPost("timkiembanbe")] // /api/s/timkiembanbe
        public IActionResult TimKiemBB([FromBody] SearchRequest req)
        {
            try
            {
                var siu = Npg.TimKiemBB(req.Username);
                if (siu != null)
                {
                    return Ok(new { siu });
                }
                return BadRequest(new { });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("danhsachbanbe")] // /api/s/danhsachbanbe
        public async Task<IActionResult> LayDanhSachBanBe([FromQuery] int maTK)
        {
            try
            {
                var bb = await Npg.LayDanhSachBanBe(maTK);
                if (bb != null)
                {
                    return Ok(bb);
                }
                return BadRequest(new { message = "Không tìm thấy thông tin bạn bè" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("loimoiketban")] // /api/s/loimoiketban
        public async Task<IActionResult> LayLoiMoiKetBan([FromQuery] int maTK)
        {
            try
            {
                var dsLoiMoi = await Npg.LayLoiMoiKetBan(maTK);
                return Ok(dsLoiMoi);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
    public class SearchRequest
    {
        public string TenTK { get; set; }
    }
}
