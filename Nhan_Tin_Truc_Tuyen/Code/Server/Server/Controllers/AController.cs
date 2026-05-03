using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")] // api/a
    [ApiController]
    public class AController : ControllerBase
    {
        [HttpGet] // /api/a
        public IActionResult Get()
        {
            return Ok("Xin chào quản trị viên!");
        }

        [HttpGet("tongtktn")] // /api/a/tongtktn
        public IActionResult TongTKTN()
        {
            try
            {
                int tk = int.Parse(Npg.TongTK());
                int tn = int.Parse(Npg.TongTN());
                return Ok(new { tk, tn });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
