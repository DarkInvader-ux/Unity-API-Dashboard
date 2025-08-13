using System.Collections.Generic;
using Core.Controllers;
using Core.Interfaces;
using UnityEngine;

namespace Core.Components
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [SerializeField] private PlayerSettings settings;
        [SerializeField] private Transform cameraTransform;
    
        private PlayerComponents components;
        private readonly List<IPlayerController> controllers = new List<IPlayerController>();

        private void Awake()
        {
            // Dependency composition
            components = new PlayerComponents
            {
                playerTransform = transform,
                cameraTransform = cameraTransform,
                characterController = GetComponent<CharacterController>(),
                settings = settings
            };

            // Adding subsystems (Open/Closed Principle)
            controllers.Add(new MovementController());
            controllers.Add(new LookController());

            // Initialize all controllers
            foreach (var controller in controllers)
            {
                controller.Initialize(components);
            }
        }

        private void Update()
        {
            foreach (var controller in controllers)
            {
                controller.Update();
            }
        }
    }
}
