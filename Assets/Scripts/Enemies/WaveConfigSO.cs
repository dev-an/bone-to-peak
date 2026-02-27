using System;
using UnityEngine;

namespace BoneToPeak.Enemies
{
    [CreateAssetMenu(menuName = "BoneToPeak/Wave Config")]
    public class WaveConfigSO : ScriptableObject
    {
        [SerializeField] private int _waveNumber = 1;
        [SerializeField] private float _duration = 30f;
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private WaveEnemyEntry[] _enemyEntries;
        [SerializeField] private float _hpMultiplier = 1f;
        [SerializeField] private float _atkMultiplier = 1f;
        [SerializeField] private float _spdMultiplier = 1f;
        [SerializeField] private float _restDuration = 10f;

        public int WaveNumber => _waveNumber;
        public float Duration => _duration;
        public float SpawnInterval => _spawnInterval;
        public WaveEnemyEntry[] EnemyEntries => _enemyEntries;
        public float HpMultiplier => _hpMultiplier;
        public float AtkMultiplier => _atkMultiplier;
        public float SpdMultiplier => _spdMultiplier;
        public float RestDuration => _restDuration;
    }

    [Serializable]
    public struct WaveEnemyEntry
    {
        public GameObject EnemyPrefab;
        public float SpawnWeight;
    }
}
