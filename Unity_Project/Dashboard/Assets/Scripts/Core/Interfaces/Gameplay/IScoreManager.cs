using Core.Gameplay;

namespace Core.Interfaces.Gameplay
{
    public interface IScoreManager
    {
        int Score { get; }
        event System.Action<int> OnScoreChanged;

        void RegisterTarget(TargetHealth targethealth);
        void AddScore(int points);
    }
}