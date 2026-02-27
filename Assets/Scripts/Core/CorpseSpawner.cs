using UnityEngine;

namespace BoneToPeak.Core
{
    public class CorpseSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _corpsePrefab;
        [SerializeField] private float _scatterRadius = 0.5f;

        private void OnEnable()
        {
            GameEvents.OnEnemyDeath += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyDeath -= HandleEnemyDeath;
        }

        private void HandleEnemyDeath(EnemyDeathEventArgs args)
        {
            if (_corpsePrefab == null) return;

            for (int i = 0; i < args.CorpseDropCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * _scatterRadius;
                Vector3 spawnPos = args.DeathPosition + new Vector3(offset.x, offset.y, 0f);

                GameObject corpseObj = ObjectPoolManager.Instance.Spawn(_corpsePrefab, spawnPos, Quaternion.identity);

                if (corpseObj.TryGetComponent<Corpse>(out var corpse))
                {
                    corpse.Initialize(1, _corpsePrefab);
                }
            }
        }
    }
}
