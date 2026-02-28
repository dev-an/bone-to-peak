using UnityEngine;

namespace BoneToPeak.Core
{
    public class Corpse : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _flashStartTime = 3.5f;
        [SerializeField] private float _flashInterval = 0.15f;

        private SpriteRenderer _spriteRenderer;
        private GameObject _sourcePrefab;
        private int _value = 1;
        private float _timer;
        private float _flashTimer;
        private bool _isSuctioning;
        private Transform _suctionTarget;
        private float _suctionSpeed;

        public int Value => _value;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _lifeTime)
            {
                ReturnToPool();
                return;
            }

            if (_isSuctioning)
            {
                if (_suctionTarget == null)
                {
                    _isSuctioning = false;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        _suctionTarget.position,
                        _suctionSpeed * Time.deltaTime
                    );
                }
                return;
            }

            if (_timer >= _flashStartTime)
            {
                _flashTimer += Time.deltaTime;
                if (_flashTimer >= _flashInterval)
                {
                    _flashTimer = 0f;
                    if (_spriteRenderer != null)
                    {
                        _spriteRenderer.enabled = !_spriteRenderer.enabled;
                    }
                }
            }
        }

        public void Initialize(int value, GameObject prefab)
        {
            _value = value;
            _sourcePrefab = prefab;
        }

        public void StartSuction(Transform target, float speed)
        {
            _isSuctioning = true;
            _suctionTarget = target;
            _suctionSpeed = speed;
        }

        public void OnSpawnFromPool()
        {
            _timer = 0f;
            _flashTimer = 0f;
            _isSuctioning = false;
            _suctionTarget = null;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = true;
            }
        }

        public void OnReturnToPool()
        {
            _isSuctioning = false;
            _suctionTarget = null;
        }

        public void ReturnToPool()
        {
            if (_sourcePrefab != null)
            {
                ObjectPoolManager.Instance.Despawn(_sourcePrefab, gameObject);
            }
        }
    }
}
