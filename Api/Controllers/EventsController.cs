using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("Events")]
    public class EventsController : ControllerBase
    {
        private static readonly List<GameEventDto> _events = new();

        [HttpGet]
        public ActionResult<IEnumerable<GameEventDto>> GetEvents()
        {
            return Ok(_events);
        }

        [HttpPost]
        public ActionResult PostEvent([FromBody] GameEventDto gameData)
        {
            if (gameData == null)
                return BadRequest("Game data cannot be null.");

            if (string.IsNullOrWhiteSpace(gameData.PlayerId))
                return BadRequest("PlayerId is required.");

            if (string.IsNullOrWhiteSpace(gameData.EventType))
                return BadRequest("EventType is required.");

            if (gameData.Position == null)
                return BadRequest("Position is required.");

            _events.Add(gameData);

            return Ok(new
            {
                message = "Game event received successfully",
                data = gameData
            });
        }
    }
}
