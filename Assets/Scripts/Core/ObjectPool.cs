using UnityEngine;
using UnityEngine.Pool;

namespace BoneToPeak.Core
{
    public class ObjectPool
    {
        private readonly ObjectPool<GameObject> _pool;
        private readonly GameObject _prefab;
        private readonly Transform _parent;

        public ObjectPool(GameObject prefab, Transform parent, int initialSize, int maxSize)
        {
            _prefab = prefab;
            _parent = parent;

            _pool = new ObjectPool<GameObject>(
                createFunc: CreateObject,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyObject,
                collectionCheck: true,
                defaultCapacity: initialSize,
                maxSize: maxSize
            );

            Prewarm(initialSize);
        }

        public GameObject Get()
        {
            return _pool.Get();
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            var obj = _pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        public void Release(GameObject obj)
        {
            _pool.Release(obj);
        }

        private GameObject CreateObject()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            return obj;
        }

        private void OnGetFromPool(GameObject obj)
        {
            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnSpawnFromPool();
            }
        }

        private void OnReleaseToPool(GameObject obj)
        {
            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnReturnToPool();
            }

            obj.SetActive(false);
        }

        private void OnDestroyObject(GameObject obj)
        {
            Object.Destroy(obj);
        }

        private void Prewarm(int count)
        {
            var objects = new GameObject[count];

            for (var i = 0; i < count; i++)
            {
                objects[i] = _pool.Get();
            }

            for (var i = 0; i < count; i++)
            {
                _pool.Release(objects[i]);
            }
        }
    }
}
