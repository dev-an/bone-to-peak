using UnityEngine;

namespace BoneToPeak.Minions
{
    [CreateAssetMenu(menuName = "BoneToPeak/Minion Stats")]
    public class MinionStatsSO : ScriptableObject
    {
        [SerializeField] private string _minionName;
        [SerializeField] private MinionTier _tier = MinionTier.T1;
        [SerializeField] private float _maxHealth = 30f;
        [SerializeField] private float _attack = 12f;
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _attackRange = 1f;
        [SerializeField] private int _corpseCost = 1;
        [SerializeField] private bool _isRanged;

        public string MinionName => _minionName;
        public MinionTier Tier => _tier;
        public float MaxHealth => _maxHealth;
        public float Attack => _attack;
        public float AttackSpeed => _attackSpeed;
        public float MoveSpeed => _moveSpeed;
        public float AttackRange => _attackRange;
        public int CorpseCost => _corpseCost;
        public bool IsRanged => _isRanged;
    }
}
