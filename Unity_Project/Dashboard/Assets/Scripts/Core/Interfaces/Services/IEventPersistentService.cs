using Core.Data;

namespace Core.Interfaces.Services
{
    public interface IEventPersistenceService
    {
        void SaveEvent(GameEventDto gameEvent);
    }
}