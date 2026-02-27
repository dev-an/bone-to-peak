using BoneToPeak.Core;
using UnityEngine;

namespace BoneToPeak.Player
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CorpseCollector : MonoBehaviour
    {
        [SerializeField] private float _suctionSpeed = 10f;
        [SerializeField] private float _collectDistance = 0.3f;

        private PlayerCombat _playerCombat;
        private Transform _playerTransform;

        private void Awake()
        {
            _playerCombat = GetComponentInParent<PlayerCombat>();
            _playerTransform = _playerCombat != null ? _playerCombat.transform : transform.parent;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Corpse>(out var corpse))
            {
                corpse.StartSuction(_playerTransform, _suctionSpeed);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent<Corpse>(out var corpse)) return;

            float distance = Vector2.Distance(other.transform.position, _playerTransform.position);

            if (distance <= _collectDistance)
            {
                int added = _playerCombat.TryAddCorpse(corpse.Value);

                if (added > 0)
                {
                    corpse.ReturnToPool();
                }
            }
        }
    }
}
