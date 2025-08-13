using System;
using System.Collections.Generic;
using Core.Data;
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
        private ITargetHitHandler hitHandler;


        private void Awake()
        {
            Position = transform.position;
        }

        public Vector3 Position { get; set; }
        
        // modify according to shooting
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out FPSController fpsController))
            {
                OnHit();
            }
        }

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
            hitHandler.HandleTargetHit(this, updatedEventDto);
        }
    }
}
