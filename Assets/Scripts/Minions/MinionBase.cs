using BoneToPeak.Core;
using BoneToPeak.Enemies;
using BoneToPeak.Player;
using UnityEngine;

namespace BoneToPeak.Minions
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MinionBase : MonoBehaviour, IDamageable, IPoolable
    {
        [SerializeField] private MinionStatsSO _stats;

        private const float DetectionRange = 6f;
        private const float FollowDistance = 2f;
        private const float MaxChaseDistance = 8f;
        private const float EnemySearchInterval = 0.3f;
        private const int MaxOverlapResults = 16;

        private Health _health;
        private Rigidbody2D _rb;
        private Transform _playerTransform;
        private GameObject _sourcePrefab;
        private MinionState _state = MinionState.Follow;
        private EnemyBase _targetEnemy;
        private float _searchTimer;
        private float _attackCooldownTimer;
        private Vector2 _followOffset;

        private static int _followIndex;
        private static int _enemyLayerMask = -1;
        private static readonly Collider2D[] OverlapResults = new Collider2D[MaxOverlapResults];

        public float CurrentHealth => _health?.CurrentHealth ?? 0f;
        public float MaxHealth => _health?.MaxHealth ?? 0f;
        public bool IsDead => _health?.IsDead ?? true;
        public float Attack => _stats.Attack;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _health = new Health(_stats.MaxHealth);
            _health.OnDeath += HandleDeath;
        }

        private void Update()
        {
            if (IsDead) return;

            if (_attackCooldownTimer > 0f)
            {
                _attackCooldownTimer -= Time.deltaTime;
            }

            switch (_state)
            {
                case MinionState.Follow:
                    UpdateFollow();
                    _searchTimer += Time.deltaTime;
                    if (_searchTimer >= EnemySearchInterval)
                    {
                        _searchTimer = 0f;
                        SearchForEnemy();
                    }
                    break;

                case MinionState.Attack:
                    UpdateAttack();
                    break;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDeath -= HandleDeath;
            }
        }

        public void Initialize(GameObject prefab)
        {
            _sourcePrefab = prefab;
            _health.Reset(_stats.MaxHealth);
            _state = MinionState.Follow;
            _targetEnemy = null;
            _attackCooldownTimer = 0f;
            _searchTimer = 0f;

            _followIndex++;
            float angle = _followIndex * 137.5f * Mathf.Deg2Rad;
            _followOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * FollowDistance;
        }

        public void OnSpawnFromPool()
        {
            if (PlayerController.Instance != null)
            {
                _playerTransform = PlayerController.Instance.transform;
            }
        }

        public void OnReturnToPool()
        {
            _rb.linearVelocity = Vector2.zero;
            _targetEnemy = null;
            _playerTransform = null;
            _state = MinionState.Follow;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            _health.TakeDamage(damage);
        }

        private void UpdateFollow()
        {
            if (_playerTransform == null) return;

            Vector2 targetPos = (Vector2)_playerTransform.position + _followOffset;
            Vector2 direction = targetPos - _rb.position;
            float distance = direction.magnitude;

            if (distance > 0.3f)
            {
                _rb.linearVelocity = direction.normalized * _stats.MoveSpeed;
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }
        }

        private void UpdateAttack()
        {
            if (_targetEnemy == null || _targetEnemy.IsDead || !_targetEnemy.gameObject.activeInHierarchy)
            {
                TransitionToFollow();
                return;
            }

            float distToPlayer = _playerTransform != null
                ? Vector2.Distance(_rb.position, (Vector2)_playerTransform.position)
                : 0f;

            if (distToPlayer > MaxChaseDistance)
            {
                TransitionToFollow();
                return;
            }

            Vector2 direction = (Vector2)_targetEnemy.transform.position - _rb.position;
            float distToEnemy = direction.magnitude;

            if (distToEnemy <= _stats.AttackRange)
            {
                _rb.linearVelocity = Vector2.zero;

                if (_attackCooldownTimer <= 0f)
                {
                    float damage = DamageCalculator.Calculate(_stats.Attack, 0f);
                    _targetEnemy.TakeDamage(damage);
                    _attackCooldownTimer = _stats.AttackInterval;
                }
            }
            else
            {
                _rb.linearVelocity = direction.normalized * _stats.MoveSpeed;
            }

            _searchTimer += Time.deltaTime;
            if (_searchTimer >= EnemySearchInterval)
            {
                _searchTimer = 0f;
                SearchForEnemy();
            }
        }

        private void SearchForEnemy()
        {
            if (_enemyLayerMask == -1)
            {
                _enemyLayerMask = LayerMask.GetMask("Enemy");
            }

            int count = Physics2D.OverlapCircleNonAlloc(
                _rb.position, DetectionRange, OverlapResults, _enemyLayerMask
            );

            float closestDist = float.MaxValue;
            EnemyBase closestEnemy = null;

            for (int i = 0; i < count; i++)
            {
                if (!OverlapResults[i].TryGetComponent<EnemyBase>(out var enemy)) continue;
                if (enemy.IsDead) continue;

                float dist = Vector2.Distance(_rb.position, OverlapResults[i].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestEnemy = enemy;
                }
            }

            if (closestEnemy != null)
            {
                _targetEnemy = closestEnemy;
                _state = MinionState.Attack;
            }
            else if (_state == MinionState.Attack)
            {
                TransitionToFollow();
            }
        }

        private void TransitionToFollow()
        {
            _state = MinionState.Follow;
            _targetEnemy = null;
        }

        private void HandleDeath()
        {
            _rb.linearVelocity = Vector2.zero;

            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.GetComponent<PlayerCombat>()?.UnregisterMinion();
            }

            Debug.Log($"[MinionBase] 미니언 사망: {_stats.MinionName}");

            if (_sourcePrefab != null)
            {
                ObjectPoolManager.Instance.Despawn(_sourcePrefab, gameObject);
            }
        }
    }
}
