using Core.Components.Gameplay;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "TargetSpawnConfig", menuName = "Config/Target Spawn Config")]
    public class TargetSpawnConfig : ScriptableObject
    {
        [Header("Spawn Settings")]
        public float spawnInterval = 2f;
        public int maxActiveTargets = 5;

        [Header("Spawn Area")]
        public Vector3 areaCenter;
        public Vector3 areaSize;

        [Header("Target Prefab")]
        public Target targetPrefab;
    }
}