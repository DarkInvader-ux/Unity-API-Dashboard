using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Models
{
    [CreateAssetMenu(fileName = "GameEventData", menuName = "Events/Game Event Data")]
    public class GameEventDataSo : ScriptableObject
    {
        [Header("Player Info")]
        public string playerId;
        public string eventType;
        
        [Header("Position")]
        public Vector3 position;

        [Header("Extra Metadata")]
        public List<string> metadataKeys = new List<string>();
        public List<string> metadataValues = new List<string>();

        public GameEventDto ToDto()
        {
            var metadataDict = new Dictionary<string, string>();
            for (int i = 0; i < Mathf.Min(metadataKeys.Count, metadataValues.Count); i++)
            {
                metadataDict[metadataKeys[i]] = metadataValues[i];
            }

            return new GameEventDto
            {
                playerId = playerId,
                eventType = eventType,
                timestamp = DateTime.UtcNow.ToString("o"),
                position = new PositionDto
                {
                    x = position.x,
                    y = position.y,
                    z = position.z
                },
                metadata = metadataDict
            };
        }
    }

    [Serializable]
    public class PositionDto
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class GameEventDto
    {
        public string playerId;
        public string eventType;
        public string timestamp;
        public PositionDto position;
        public Dictionary<string, string> metadata;
    }
}