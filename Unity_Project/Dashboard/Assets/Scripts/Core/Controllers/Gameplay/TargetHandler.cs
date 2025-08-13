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
        private readonly IEventsService eventsService;
        private readonly ILogger logger;
        
        [Inject]
        private CoroutineRunner coroutineRunner;

        public TargetHitHandler(
            IEventsService eventsService,
            ILogger logger)
        {
            this.eventsService = eventsService;
            this.logger = logger;
        }

        public void HandleTargetHit(ITarget target, GameEventDto gameEvent)
        {
            coroutineRunner.RunCoroutine(eventsService.PostEvent(gameEvent));
        }
    }

}

