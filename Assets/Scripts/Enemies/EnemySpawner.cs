using System.Collections;
using BoneToPeak.Core;
using BoneToPeak.Player;
using UnityEngine;

namespace BoneToPeak.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private WaveConfigSO[] _waveConfigs;
        [SerializeField] private float _spawnOffsetFromCamera = 1.5f;

        private Camera _mainCamera;
        private int _currentWaveIndex;
        private bool _isSpawning;

        private void Start()
        {
            _mainCamera = Camera.main;

            if (_waveConfigs == null || _waveConfigs.Length == 0)
            {
                Debug.LogError("[EnemySpawner] WaveConfig가 할당되지 않았습니다.", this);
                return;
            }

            StartCoroutine(WaveLoop());
        }

        private IEnumerator WaveLoop()
        {
            while (_currentWaveIndex < _waveConfigs.Length)
            {
                var config = _waveConfigs[_currentWaveIndex];
                GameEvents.RaiseWaveStarted(config.WaveNumber);
                Debug.Log($"[EnemySpawner] === Wave {config.WaveNumber} 시작 ===");

                _isSpawning = true;
                float elapsed = 0f;
                float spawnTimer = 0f;

                while (elapsed < config.Duration)
                {
                    spawnTimer += Time.deltaTime;
                    elapsed += Time.deltaTime;

                    if (spawnTimer >= config.SpawnInterval)
                    {
                        SpawnEnemy(config);
                        spawnTimer = 0f;
                    }

                    yield return null;
                }

                _isSpawning = false;
                GameEvents.RaiseWaveEnded(config.WaveNumber);
                Debug.Log($"[EnemySpawner] === Wave {config.WaveNumber} 종료 === 휴식: {config.RestDuration}초");

                _currentWaveIndex++;

                if (_currentWaveIndex < _waveConfigs.Length)
                {
                    yield return new WaitForSeconds(config.RestDuration);
                }
            }

            Debug.Log("[EnemySpawner] 모든 웨이브 완료!");
        }

        private void SpawnEnemy(WaveConfigSO config)
        {
            GameObject prefab = SelectEnemyPrefab(config);
            if (prefab == null) return;

            Vector3 spawnPosition = GetSpawnPosition();
            GameObject enemyObj = ObjectPoolManager.Instance.Spawn(prefab, spawnPosition, Quaternion.identity);

            if (enemyObj.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.Initialize(config.HpMultiplier, config.AtkMultiplier, config.SpdMultiplier, prefab);
            }
        }

        private GameObject SelectEnemyPrefab(WaveConfigSO config)
        {
            var entries = config.EnemyEntries;
            if (entries == null || entries.Length == 0) return null;

            float totalWeight = 0f;
            foreach (var entry in entries)
            {
                totalWeight += entry.SpawnWeight;
            }

            float random = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var entry in entries)
            {
                cumulative += entry.SpawnWeight;
                if (random <= cumulative)
                {
                    return entry.EnemyPrefab;
                }
            }

            return entries[entries.Length - 1].EnemyPrefab;
        }

        private Vector3 GetSpawnPosition()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            float halfHeight = _mainCamera.orthographicSize + _spawnOffsetFromCamera;
            float halfWidth = halfHeight * _mainCamera.aspect;
            Vector3 cameraPos = _mainCamera.transform.position;

            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            float x, y;

            if (Mathf.Abs(cos) * halfHeight > Mathf.Abs(sin) * halfWidth)
            {
                x = Mathf.Sign(cos) * halfWidth;
                y = sin / Mathf.Abs(cos) * halfHeight;
            }
            else
            {
                y = Mathf.Sign(sin) * halfHeight;
                x = cos / Mathf.Abs(sin) * halfWidth;
            }

            return new Vector3(cameraPos.x + x, cameraPos.y + y, 0f);
        }
    }
}
