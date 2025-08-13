using UnityEngine;

namespace Core.Interfaces
{
    public interface IPlayerController
    {
        void Initialize(PlayerComponents components);
        void Update();
    }
    // Player Components Container (Dependency Aggregation)
    [System.Serializable]
    public class PlayerComponents
    {
        public Transform playerTransform;
        public Transform cameraTransform;
        public CharacterController characterController;
        public PlayerSettings settings;
    }

// Player Settings (Data Container)
    [System.Serializable]
    public class PlayerSettings
    {
        [Header("Movement")]
        public float walkSpeed = 5f;
        public float sprintSpeed = 8f;
        public float crouchSpeed = 2.5f;
        public float jumpHeight = 1.5f;
        public float gravity = -9.81f;

        [Header("Look")]
        public float mouseSensitivity = 100f;
        public float maxVerticalAngle = 90f;

        [Header("Crouch")]
        public float crouchHeight = 1f;
        public float standHeight = 2f;
        public float heightSmoothTime = 0.1f;
    }
}
