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

        //[HttpPost("timkiembb")] // /api/s/timkiembb
        //public IActionResult TimKiemBB([FromBody] SearchRequest req)
        //{
        //    try
        //    {
        //        var siu = Npg.TimKiemBB(req.Username);
        //        if (siu.Count > 0)
        //        {
        //            return Ok(new { siu });
        //        }
        //        return BadRequest( new {});
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(new { message = e.Message });
        //    }
        //}
    }
    public class SearchRequest
    {
        public string Username { get; set; }
    }
}
