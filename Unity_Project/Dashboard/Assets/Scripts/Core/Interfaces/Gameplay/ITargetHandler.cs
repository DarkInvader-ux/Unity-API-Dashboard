using Core.Data;
using Core.Models;

namespace Core.Interfaces.Gameplay
{
    public interface ITargetHitHandler
    {
        void HandleTargetHit(ITarget target, GameEventDto gameEventDto);
    }
}

