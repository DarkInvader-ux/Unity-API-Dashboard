using System.Collections;
using Core.Components.Gameplay;
using Core.Data;
using Core.Interfaces.Gameplay;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public class TargetSpawner : ITargetSpawner, IInitializable
    {
        private readonly ICoroutineRunner coroutineRunner;
        private readonly TargetSpawnConfig config;
        private readonly Target.Factory targetFactory;

        private Coroutine spawnCoroutine;
        private int activeTargetCount;

        [Inject]
        public TargetSpawner(CoroutineRunner coroutineRunner, TargetSpawnConfig config, Target.Factory targetFactory)
        {
            this.coroutineRunner = coroutineRunner;
            this.config = config;
            this.targetFactory = targetFactory;
        }
        
        

        public void StartSpawning()
        {
            spawnCoroutine = coroutineRunner.RunCoroutine(SpawnLoop());
        }

        public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                coroutineRunner.StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                if (activeTargetCount < config.maxActiveTargets)
                {
                    SpawnTarget();
                }
                yield return new WaitForSeconds(config.spawnInterval);
            }
        }

        private void SpawnTarget()
        {
            Vector3 spawnPos = GetRandomPosition();
            Target target = targetFactory.Create();
            target.transform.position = spawnPos;
            activeTargetCount++;

            target.OnDestroyed += () => activeTargetCount--; // Listen for destruction
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 halfSize = config.areaSize / 2f;
            return config.areaCenter + new Vector3(
                Random.Range(-halfSize.x, halfSize.x),
                Random.Range(-halfSize.y, halfSize.y),
                Random.Range(-halfSize.z, halfSize.z)
            );
        }

        public void Initialize()
        {
            StartSpawning();
        }
    }
}