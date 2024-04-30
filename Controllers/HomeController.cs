using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        // health check
        [HttpGet("")]
        public IActionResult Get() => Ok();

        [HttpGet("/test")]
        [ApiKey]
        public IActionResult TestApiKey() => Ok();
    }
}
