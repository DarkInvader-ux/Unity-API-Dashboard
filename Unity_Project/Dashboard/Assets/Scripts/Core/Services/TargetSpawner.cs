using System.Collections;
using Core.Components.Gameplay;
using Core.Data;
using Core.Gameplay;
using Core.Interfaces.Gameplay;
using Core.Utilities;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public class TargetSpawner : ITargetSpawner, IInitializable
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly TargetSpawnConfig _config;
        private readonly Target.Factory _targetFactory;
        private readonly IScoreManager _scoreManager;

        private Coroutine _spawnCoroutine;
        private int _activeTargetCount;

        private readonly TargetPool _targetPool;

        [Inject]
        public TargetSpawner(CoroutineRunner coroutineRunner, TargetSpawnConfig config, TargetPool targetPool, IScoreManager scoreManager)
        {
            _coroutineRunner = coroutineRunner;
            _config = config;
            _targetPool = targetPool;
            _scoreManager = scoreManager;
        }

        private void SpawnTarget()
        {
            Vector3 spawnPos = GetRandomPosition();
            Target target = _targetPool.Spawn(spawnPos);

            if (target.TryGetComponent<TargetHealth>(out var targetHealth))
            {
                _scoreManager.RegisterTarget(targetHealth);
                _activeTargetCount++;
                target.OnDestroyed += () => 
                {
                    _targetPool.Despawn(target);
                    _activeTargetCount--;
                };
            }
        }

        
        

        public void StartSpawning()
        {
            _spawnCoroutine = _coroutineRunner.RunCoroutine(SpawnLoop());
        }

        public void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                if (_activeTargetCount < _config.maxActiveTargets)
                {
                    SpawnTarget();
                }
                yield return new WaitForSeconds(_config.spawnInterval);
            }
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 halfSize = _config.areaSize / 2f;
            return _config.areaCenter + new Vector3(
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