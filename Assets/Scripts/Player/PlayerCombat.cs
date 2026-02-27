using System.Collections;
using BoneToPeak.Core;
using UnityEngine;

namespace BoneToPeak.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        [Header("체력")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _defense;
        [SerializeField] private float _invulnerabilityDuration = 0.5f;

        [Header("시체")]
        [SerializeField] private int _maxCorpseCapacity = 10;

        [Header("소환")]
        [SerializeField] private int _maxSummonSlots = 3;
        [SerializeField] private float _summonCooldown = 2f;

        private Health _health;
        private SpriteRenderer _spriteRenderer;
        private int _currentCorpseCount;
        private int _currentMinionCount;
        private float _invulnerabilityTimer;
        private float _summonCooldownTimer;
        private float _contactDamageTimer;

        private const float ContactDamageInterval = 0.5f;

        public float CurrentHealth => _health.CurrentHealth;
        public float MaxHealth => _health.MaxHealth;
        public bool IsDead => _health.IsDead;
        public int CurrentCorpseCount => _currentCorpseCount;
        public int MaxCorpseCapacity => _maxCorpseCapacity;
        public int CurrentMinionCount => _currentMinionCount;
        public int MaxSummonSlots => _maxSummonSlots;
        public bool CanSummon => _summonCooldownTimer <= 0f && _currentMinionCount < _maxSummonSlots;

        private void Awake()
        {
            _health = new Health(_maxHealth);
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _health.OnHealthChanged += HandleHealthChanged;
            _health.OnDeath += HandleDeath;
        }

        private void Update()
        {
            if (_invulnerabilityTimer > 0f)
            {
                _invulnerabilityTimer -= Time.deltaTime;
            }

            if (_summonCooldownTimer > 0f)
            {
                _summonCooldownTimer -= Time.deltaTime;
            }

            if (_contactDamageTimer > 0f)
            {
                _contactDamageTimer -= Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            _health.OnHealthChanged -= HandleHealthChanged;
            _health.OnDeath -= HandleDeath;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead || _invulnerabilityTimer > 0f) return;

            _health.TakeDamage(damage);
            _invulnerabilityTimer = _invulnerabilityDuration;
            StartCoroutine(InvulnerabilityFlash());
        }

        public int TryAddCorpse(int amount)
        {
            int space = _maxCorpseCapacity - _currentCorpseCount;
            int added = Mathf.Min(amount, space);

            if (added <= 0) return 0;

            _currentCorpseCount += added;
            GameEvents.RaiseCorpseCountChanged(_currentCorpseCount);
            Debug.Log($"[PlayerCombat] 시체 수집: +{added} (보유: {_currentCorpseCount}/{_maxCorpseCapacity})");
            return added;
        }

        public bool TryConsumeCorpse(int amount)
        {
            if (_currentCorpseCount < amount) return false;

            _currentCorpseCount -= amount;
            GameEvents.RaiseCorpseCountChanged(_currentCorpseCount);
            Debug.Log($"[PlayerCombat] 시체 소비: -{amount} (보유: {_currentCorpseCount}/{_maxCorpseCapacity})");
            return true;
        }

        public void RegisterMinion()
        {
            _currentMinionCount++;
            GameEvents.RaiseMinionCountChanged(_currentMinionCount, _maxSummonSlots);
            Debug.Log($"[PlayerCombat] 미니언 등록: {_currentMinionCount}/{_maxSummonSlots}");
        }

        public void UnregisterMinion()
        {
            _currentMinionCount = Mathf.Max(0, _currentMinionCount - 1);
            GameEvents.RaiseMinionCountChanged(_currentMinionCount, _maxSummonSlots);
            Debug.Log($"[PlayerCombat] 미니언 해제: {_currentMinionCount}/{_maxSummonSlots}");
        }

        public void StartSummonCooldown()
        {
            _summonCooldownTimer = _summonCooldown;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsDead || _contactDamageTimer > 0f) return;

            if (!other.TryGetComponent<Enemies.EnemyBase>(out var enemy)) return;
            if (enemy.IsDead) return;

            float damage = DamageCalculator.Calculate(enemy.Attack, _defense);
            TakeDamage(damage);
            _contactDamageTimer = ContactDamageInterval;
        }

        private void HandleHealthChanged(float current, float max)
        {
            GameEvents.RaisePlayerHealthChanged(current / max);
        }

        private void HandleDeath()
        {
            GameEvents.RaisePlayerDeath();
            Debug.Log("[PlayerCombat] 플레이어 사망!");
        }

        private IEnumerator InvulnerabilityFlash()
        {
            if (_spriteRenderer == null) yield break;

            float elapsed = 0f;
            const float flashInterval = 0.1f;

            while (elapsed < _invulnerabilityDuration)
            {
                Color color = _spriteRenderer.color;
                color.a = color.a < 1f ? 1f : 0.3f;
                _spriteRenderer.color = color;

                yield return new WaitForSeconds(flashInterval);
                elapsed += flashInterval;
            }

            Color finalColor = _spriteRenderer.color;
            finalColor.a = 1f;
            _spriteRenderer.color = finalColor;
        }
    }
}
