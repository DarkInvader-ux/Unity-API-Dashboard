using Core.Components.Gameplay;
using Core.Data;
using Core.Interfaces;
using Core.Interfaces.Gameplay;
using Core.Interfaces.Services;
using Core.Models;
using ILogger = Core.Interfaces.Api.ILogger;
using Zenject;

namespace Core.Controllers.Gameplay
{
    public class TargetHitHandler : ITargetHitHandler
    {
        private readonly IEventsService _eventsService;
        private readonly IEventPersistenceService _eventPersistenceService;
        [Inject]
        private CoroutineRunner _coroutineRunner;

        public TargetHitHandler(IEventsService eventsService, IEventPersistenceService eventPersistenceService)
        {
            _eventsService = eventsService;
            _eventPersistenceService = eventPersistenceService;
        }

        public void HandleTargetHit(ITarget target, GameEventDto gameEvent)
        {
            _coroutineRunner.RunCoroutine(_eventsService.PostEvent(gameEvent));
            _eventPersistenceService.SaveEvent(gameEvent);
        }
    }

}

