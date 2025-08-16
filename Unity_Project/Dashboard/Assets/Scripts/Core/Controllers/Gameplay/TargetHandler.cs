using Core.Components.Gameplay;
using Core.Data;
using Core.Interfaces;
using Core.Interfaces.Gameplay;
using Core.Models;
using ILogger = Core.Interfaces.Api.ILogger;
using Zenject;

namespace Core.Controllers.Gameplay
{
    public class TargetHitHandler : ITargetHitHandler
    {
        private readonly IEventsService _eventsService;
        private readonly ILogger _logger;
        
        [Inject]
        private CoroutineRunner _coroutineRunner;

        public TargetHitHandler(IEventsService eventsService, ILogger logger)
        {
            _eventsService = eventsService;
            _logger = logger;
        }

        public void HandleTargetHit(ITarget target, GameEventDto gameEvent)
        {
            _coroutineRunner.RunCoroutine(_eventsService.PostEvent(gameEvent));
        }
    }

}

