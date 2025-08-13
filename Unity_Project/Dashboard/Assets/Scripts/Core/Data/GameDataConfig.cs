using UnityEngine;

namespace Core.Models
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "Events API/Game Data")]
    public class GameDataConfig : ScriptableObject
    {
        public string playerName;
        public int score;
        public string level;
        public string additionalInfo;

        public GameDataDTO GameDataDto()
        {
            return new GameDataDTO
            {
                PlayerName = playerName,
                Score = score,
                Level = level,
                AdditionalInfo = additionalInfo
            };
        }
    }

    [System.Serializable]
    public class GameDataDTO
    {
        public string PlayerName;
        public int Score;
        public string Level;
        public string AdditionalInfo;
    }
}