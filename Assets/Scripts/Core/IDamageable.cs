namespace BoneToPeak.Core
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }
        void TakeDamage(float damage);
    }
}
