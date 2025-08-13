using Core.Interfaces;
using UnityEngine;

namespace Core.Controllers
{
    public class MovementController : IPlayerController
    {
        private PlayerComponents components;
        private Vector3 verticalVelocity;
        private bool isGrounded;
        private bool isCrouching;

        public void Initialize(PlayerComponents comps) => components = comps;

        public void Update()
        {
            HandleMovement();
            HandleJump();
            HandleCrouch();
            ApplyGravity();
        }

        private void HandleMovement()
        {
            float speed = GetTargetSpeed();
            Vector3 move = components.playerTransform.right * Input.GetAxis("Horizontal") +
                           components.playerTransform.forward * Input.GetAxis("Vertical");
            components.characterController.Move(speed * Time.deltaTime * move);
        }

        private float GetTargetSpeed()
        {
            if (isCrouching) return components.settings.crouchSpeed;
            return Input.GetKey(KeyCode.LeftShift) ? components.settings.sprintSpeed : components.settings.walkSpeed;
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(components.settings.jumpHeight * -2f * components.settings.gravity);
            }
        }

        private void HandleCrouch()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCrouching = !isCrouching;
                float targetHeight = isCrouching ? components.settings.crouchHeight : components.settings.standHeight;
                components.characterController.height = Mathf.Lerp(
                    components.characterController.height,
                    targetHeight,
                    components.settings.heightSmoothTime * Time.deltaTime
                );
            }
        }

        private void ApplyGravity()
        {
            isGrounded = components.characterController.isGrounded;
            if (isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }

            verticalVelocity.y += components.settings.gravity * Time.deltaTime;
            components.characterController.Move(verticalVelocity * Time.deltaTime);
        }
    }
}