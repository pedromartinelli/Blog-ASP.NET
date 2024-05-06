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
        public IActionResult Get(IConfiguration config)
        {
            var env = config.GetValue<string>("Env");

            return Ok(env);
        }

        [HttpGet("/test")]
        [ApiKey]
        public IActionResult TestApiKey() => Ok();
    }
}
