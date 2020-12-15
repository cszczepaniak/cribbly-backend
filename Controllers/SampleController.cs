using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return await Task.Run(() =>
            {
                return Ok("Hello, cribbly!");
            });
        }
    }
}
