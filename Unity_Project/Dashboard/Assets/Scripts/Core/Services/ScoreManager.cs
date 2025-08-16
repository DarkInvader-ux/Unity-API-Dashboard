using System;
using Core.Gameplay;
using Core.Interfaces.Gameplay;
using UnityEngine;

namespace Core.Services
{
    public class ScoreManager : IScoreManager
    {
        public int Score { get; private set; }
        public event Action<int> OnScoreChanged;


        public void RegisterTarget(TargetHealth targethealth)
        {
            targethealth.OnDeath += () => AddScore(10);
        }

        public void AddScore(int points)
        {
            Score += points;
            Debug.Log($"Score: {Score}");
            OnScoreChanged?.Invoke(Score);
        }
    }
}