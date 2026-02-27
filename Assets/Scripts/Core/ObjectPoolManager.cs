using System.Collections.Generic;
using UnityEngine;

namespace BoneToPeak.Core
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [SerializeField] private int _defaultInitialSize = 10;
        [SerializeField] private int _defaultMaxSize = 100;

        private readonly Dictionary<GameObject, ObjectPool> _pools = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public GameObject Spawn(GameObject prefab)
        {
            return GetOrCreatePool(prefab).Get();
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return GetOrCreatePool(prefab).Get(position, rotation);
        }

        public void Despawn(GameObject prefab, GameObject obj)
        {
            if (_pools.TryGetValue(prefab, out var pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogError($"[ObjectPoolManager] '{prefab.name}' 프리팹에 대한 풀이 존재하지 않습니다.");
            }
        }

        private ObjectPool GetOrCreatePool(GameObject prefab)
        {
            if (_pools.TryGetValue(prefab, out var pool))
            {
                return pool;
            }

            var parentObj = new GameObject($"Pool_{prefab.name}");
            parentObj.transform.SetParent(transform);

            pool = new ObjectPool(prefab, parentObj.transform, _defaultInitialSize, _defaultMaxSize);
            _pools[prefab] = pool;

            return pool;
        }
    }
}
