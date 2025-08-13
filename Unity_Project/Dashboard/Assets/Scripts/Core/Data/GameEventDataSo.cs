using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
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

        public Dictionary<string, string> MetadataValuesDict = new Dictionary<string, string>();

        public GameEventDto ToDto(TargetHitDto targetHit = null)
        {
            if (targetHit != null)
            {
                // Target hit case
                MetadataValuesDict["TargetId"] = targetHit.targetId;
                MetadataValuesDict["HitTime"] = targetHit.hitTime.ToString("F2");
                MetadataValuesDict["Accuracy"] = targetHit.accuracy.ToString("F2");

                return new GameEventDto
                {
                    playerId = targetHit.playerId ?? playerId,
                    eventType = "TargetHit",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    position = targetHit.hitPosition,
                    metadata = MetadataValuesDict
                };
            }
            else
            {
                // Test case using SO variables
                for (int i = 0; i < Mathf.Min(metadataKeys.Count, metadataValues.Count); i++)
                {
                    MetadataValuesDict[metadataKeys[i]] = metadataValues[i];
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
                    metadata = MetadataValuesDict
                };
            }
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
    [Serializable]
    public class TargetHitDto
    {
        public string playerId;
        public string targetId;
        public float hitTime;
        public PositionDto hitPosition;
        public float accuracy; // optional, if you want scoring
    }
}