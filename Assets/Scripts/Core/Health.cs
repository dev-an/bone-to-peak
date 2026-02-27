using System;
using UnityEngine;

namespace BoneToPeak.Core
{
    public class Health
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        public Health(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead) return;

            float previousHealth = CurrentHealth;
            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;

            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void Reset(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}
