using BoneToPeak.Core;
using BoneToPeak.Minions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BoneToPeak.Player
{
    public class SummonSystem : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private MinionStatsSO _defaultMinionStats;
        [SerializeField] private GameObject _defaultMinionPrefab;
        [SerializeField] private float _summonRadius = 1.5f;

        private PlayerCombat _playerCombat;
        private InputAction _summonAction;

        private void Awake()
        {
            _playerCombat = GetComponent<PlayerCombat>();

            if (_inputActions == null)
            {
                Debug.LogError("[SummonSystem] InputActionAsset이 할당되지 않았습니다.", this);
                return;
            }

            _summonAction = _inputActions.FindActionMap("Player")?.FindAction("Summon");

            if (_summonAction == null)
            {
                Debug.LogError("[SummonSystem] 'Player/Summon' 액션을 찾을 수 없습니다.", this);
            }
        }

        private void OnEnable()
        {
            if (_summonAction != null)
            {
                _summonAction.Enable();
                _summonAction.performed += OnSummonPerformed;
            }
        }

        private void OnDisable()
        {
            if (_summonAction != null)
            {
                _summonAction.performed -= OnSummonPerformed;
                _summonAction.Disable();
            }
        }

        private void OnSummonPerformed(InputAction.CallbackContext context)
        {
            TrySummon();
        }

        private void TrySummon()
        {
            if (_playerCombat == null || _defaultMinionStats == null || _defaultMinionPrefab == null) return;

            if (!_playerCombat.CanSummon)
            {
                Debug.Log("[SummonSystem] 소환 쿨다운 중이거나 슬롯이 가득 찼습니다.");
                return;
            }

            if (!_playerCombat.TryConsumeCorpse(_defaultMinionStats.CorpseCost))
            {
                Debug.Log($"[SummonSystem] 시체가 부족합니다. (필요: {_defaultMinionStats.CorpseCost}, 보유: {_playerCombat.CurrentCorpseCount})");
                return;
            }

            Vector2 offset = Random.insideUnitCircle.normalized * _summonRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);

            GameObject minionObj = ObjectPoolManager.Instance.Spawn(_defaultMinionPrefab, spawnPos, Quaternion.identity);

            if (minionObj.TryGetComponent<MinionBase>(out var minion))
            {
                minion.Initialize(_defaultMinionPrefab);
            }

            _playerCombat.RegisterMinion();
            _playerCombat.StartSummonCooldown();

            Debug.Log($"[SummonSystem] {_defaultMinionStats.MinionName} 소환 완료!");
        }
    }
}
