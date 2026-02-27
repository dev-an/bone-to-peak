using System.Collections;
using BoneToPeak.Core;
using BoneToPeak.Player;
using UnityEngine;

namespace BoneToPeak.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyBase : MonoBehaviour, IDamageable, IPoolable
    {
        [SerializeField] private EnemyStatsSO _stats;

        private Health _health;
        private Rigidbody2D _rb;
        private Transform _playerTransform;
        private GameObject _sourcePrefab;
        private float _currentAttack;
        private float _currentMoveSpeed;
        private bool _isDying;

        private const float DeathDelay = 0.1f;

        public float CurrentHealth => _health?.CurrentHealth ?? 0f;
        public float MaxHealth => _health?.MaxHealth ?? 0f;
        public bool IsDead => _health?.IsDead ?? true;
        public float Attack => _currentAttack;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _health = new Health(_stats.MaxHealth);
            _health.OnDeath += HandleDeath;
        }

        private void FixedUpdate()
        {
            if (_isDying || IsDead || _playerTransform == null) return;

            Vector2 direction = ((Vector2)_playerTransform.position - _rb.position).normalized;
            _rb.linearVelocity = direction * _currentMoveSpeed;
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDeath -= HandleDeath;
            }
        }

        public void Initialize(float hpMultiplier, float atkMultiplier, float spdMultiplier, GameObject prefab)
        {
            _sourcePrefab = prefab;
            _health.Reset(_stats.MaxHealth * hpMultiplier);
            _currentAttack = _stats.Attack * atkMultiplier;
            _currentMoveSpeed = _stats.MoveSpeed * spdMultiplier;
            _isDying = false;
        }

        public void OnSpawnFromPool()
        {
            _isDying = false;

            if (PlayerController.Instance != null)
            {
                _playerTransform = PlayerController.Instance.transform;
            }
        }

        public void OnReturnToPool()
        {
            _rb.linearVelocity = Vector2.zero;
            _playerTransform = null;
            _isDying = false;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead || _isDying) return;

            _health.TakeDamage(damage);
        }

        private void HandleDeath()
        {
            if (_isDying) return;
            _isDying = true;

            _rb.linearVelocity = Vector2.zero;

            GameEvents.RaiseEnemyDeath(new EnemyDeathEventArgs
            {
                DeathPosition = transform.position,
                CorpseDropCount = _stats.CorpseDropCount,
                EnemyPrefab = _sourcePrefab,
                EnemyInstance = gameObject
            });

            StartCoroutine(DespawnAfterDelay());
        }

        private IEnumerator DespawnAfterDelay()
        {
            yield return new WaitForSeconds(DeathDelay);

            if (_sourcePrefab != null)
            {
                ObjectPoolManager.Instance.Despawn(_sourcePrefab, gameObject);
            }
        }
    }
}
