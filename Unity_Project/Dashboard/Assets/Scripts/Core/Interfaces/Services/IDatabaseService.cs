using Core.Data;

namespace Core.Interfaces.Services
{
    public interface IDatabaseService
    {
        void SaveEvent(GameEventDto gameEvent);
        void SaveScore(string playerId, int score);
    }
}
