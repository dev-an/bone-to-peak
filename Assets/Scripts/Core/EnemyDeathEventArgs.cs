using UnityEngine;

namespace BoneToPeak.Core
{
    public struct EnemyDeathEventArgs
    {
        public Vector3 DeathPosition;
        public int CorpseDropCount;
        public GameObject EnemyPrefab;
        public GameObject EnemyInstance;
    }
}
