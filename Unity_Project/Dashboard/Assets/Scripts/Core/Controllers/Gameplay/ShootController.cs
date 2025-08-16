using Core.Interfaces;
using Core.Interfaces.Gameplay;
using UnityEngine;

namespace Core.Controllers.Gameplay
{
    public class ShootController : IPlayerController
    {
        private PlayerComponents components;

        public void Initialize(PlayerComponents components)
        {
            this.components = components;
        }

        public void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = new Ray(components.cameraTransform.position, components.cameraTransform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.TryGetComponent<ITarget>(out var target))
                    {
                        target.OnHit();
                    }
                }
            }
        }
    }
}