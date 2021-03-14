using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.Run(() =>
            {
                return Ok("Hello, cribbly!");
            });
        }
        [HttpGet("private")]
        [Authorize]
        public async Task<IActionResult> GetPrivate()
        {
            return await Task.Run(() =>
            {
                return Ok("Hello, private cribbly!");
            });
        }
    }
}
