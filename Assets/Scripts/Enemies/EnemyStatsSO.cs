using UnityEngine;

namespace BoneToPeak.Enemies
{
    [CreateAssetMenu(menuName = "BoneToPeak/Enemy Stats")]
    public class EnemyStatsSO : ScriptableObject
    {
        [SerializeField] private string _enemyName;
        [SerializeField] private float _maxHealth = 15f;
        [SerializeField] private float _attack = 5f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _attackRange = 0.8f;
        [SerializeField] private int _corpseDropCount = 1;

        public string EnemyName => _enemyName;
        public float MaxHealth => _maxHealth;
        public float Attack => _attack;
        public float MoveSpeed => _moveSpeed;
        public float AttackRange => _attackRange;
        public int CorpseDropCount => _corpseDropCount;
    }
}
