using Core.Components.Gameplay;
using UnityEngine;
using Zenject;

namespace Core.Utilities
{
    public class TargetPool : MonoMemoryPool<Vector3, Target>
    {
        protected override void Reinitialize(Vector3 position, Target item)
        {
            item.transform.position = position;
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(Target item)
        {
            item.gameObject.SetActive(false);
        }
    }
}