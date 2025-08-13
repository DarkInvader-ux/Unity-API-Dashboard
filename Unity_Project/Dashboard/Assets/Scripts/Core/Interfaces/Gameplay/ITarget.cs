using Core.Models;
using UnityEngine;

namespace Core.Interfaces.Gameplay
{
    public interface ITarget
    {
        public Vector3 Position { get; set; }
        void OnHit();
    }
}
