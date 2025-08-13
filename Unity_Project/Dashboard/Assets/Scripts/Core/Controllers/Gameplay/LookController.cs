using Core.Interfaces;
using UnityEngine;

namespace Core.Controllers
{
    public class LookController : IPlayerController
    {
        private PlayerComponents components;
        private float xRotation = 0f;

        public void Initialize(PlayerComponents comps)
        {
            components = comps;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Update()
        {
            HandleMouseLook();
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * components.settings.mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * components.settings.mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -components.settings.maxVerticalAngle, components.settings.maxVerticalAngle);

            components.cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            components.playerTransform.Rotate(Vector3.up * mouseX);
        }
    }
}
