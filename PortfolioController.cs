using Microsoft.AspNetCore.Mvc;

namespace DotNet_portfolio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var data = new { Message = "Hello from .NET Backend!" };
            return Ok(data);
        }
    }
}