using System;
using System.Collections.Generic;
using Core.Data;
using Core.Gameplay;
using Core.Interfaces.Gameplay;
using Core.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core.Components.Gameplay
{
    public class Target : MonoBehaviour, ITarget
    {
        [FormerlySerializedAs("gameEventDtoSO")] [FormerlySerializedAs("gameEventDto")] [FormerlySerializedAs("_gameEventDto")] [SerializeField]
        private GameEventDataSo gameEventDtoSo;
        [Inject]
        private ITargetHitHandler _hitHandler;
        
        private TargetHealth _targetHealth;

        public event Action OnDestroyed;

        public class Factory : PlaceholderFactory<Target> { }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }
        private void Awake()
        {
            Position = transform.position;
            _targetHealth = GetComponent<TargetHealth>();
            if (_targetHealth != null)
            {
                _targetHealth.OnDeath += () => OnDestroyed?.Invoke();
            }
        }

        public Vector3 Position { get; set; }

        public void OnHit()
        {
            var updatedEventDto = new GameEventDto
            {
                playerId = gameEventDtoSo.playerId,
                eventType = "Hit",
                timestamp = DateTime.UtcNow.ToString("o"),
                position = new PositionDto
                {
                    x = Position.x,
                    y = Position.y,
                    z = Position.z,
                },
                metadata = gameEventDtoSo.MetadataValuesDict
            };
            // Just forwards the hit event to the handler
            _hitHandler.HandleTargetHit(this, updatedEventDto);
            
            _targetHealth?.ApplyDamage(1);

        }
    }
}
