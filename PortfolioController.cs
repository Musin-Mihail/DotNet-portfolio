using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNet_portfolio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(ILogger<PortfolioController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Получен запрос к /portfolio");
            var data = new { Message = "Hello from .NET Backend!" };
            return Ok(data);
        }
    }
}