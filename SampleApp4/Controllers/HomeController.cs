using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Numerics;

namespace SampleApp4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetStudents()
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? "N/A"; // Get the current trace ID
            _logger.LogInformation("Seri Log is Working | TraceId: {traceId}", traceId);

            
            _logger.LogDebug("This is a {severityLevel} message", LogLevel.Debug);
            _logger.LogInformation("{severityLevel} messages are used to provide contextual information", LogLevel.Information);
            _logger.LogError(new Exception("Application exception"), "These are usually accompanied by an exception");

            return new List<string>()
            {
                "Suresh",
                "Sajin",
                "Veeri",
                "Waqar",
                "Naveen",
                "Guru"
            };
        }
    }
}
