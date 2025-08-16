using System;
using UnityEngine;

namespace Core.Gameplay
{
    public class TargetHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 1;
        private int _currentHealth;

        public event Action OnDeath;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void ApplyDamage(int amount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= amount;

            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}