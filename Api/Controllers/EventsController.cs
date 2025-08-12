using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private static readonly List<string> _events = new();

        [HttpPost]
        public IActionResult PostEvent([FromBody] string gameEvent)
        {
            _events.Add(gameEvent);
            return Ok(new { message = "Event logged", totalEvents = _events.Count });
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_events);
        }
    }
}
