using System;
using Core.Interfaces.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using Zenject;

namespace Core.Components.UI
{
    public class ScoreUIController : MonoBehaviour
    {
        [FormerlySerializedAs("_scoreText")] [SerializeField] private TextMeshProUGUI scoreText;
        
        [Inject]
        private IScoreManager _scoreManager;

        private void OnEnable()
        {
            _scoreManager.OnScoreChanged += OnScoreChanged;
        }

        private void OnScoreChanged(int score)
        {
            scoreText.text = score.ToString();
        }

    }
}
