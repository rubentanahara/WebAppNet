using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private const string msg = "Hello, from ASP.NET Core";

        [HttpGet(Name = "HelloWorld")]
        public string Get()
        {
            return msg;
        }
    }
}
