using UnityEngine;
using UnityEngine.InputSystem;

namespace BoneToPeak.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private float _moveSpeed = 5.0f;

        private Rigidbody2D _rb;
        private InputAction _moveAction;
        private Vector2 _moveInput;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _rb = GetComponent<Rigidbody2D>();

            if (_inputActions == null)
            {
                Debug.LogError("[PlayerController] InputActionAsset이 할당되지 않았습니다.", this);
                return;
            }

            _moveAction = _inputActions.FindActionMap("Player")?.FindAction("Move");

            if (_moveAction == null)
            {
                Debug.LogError("[PlayerController] 'Player/Move' 액션을 찾을 수 없습니다.", this);
            }
        }

        private void OnEnable()
        {
            _moveAction?.Enable();
        }

        private void Update()
        {
            if (_moveAction == null) return;

            _moveInput = _moveAction.ReadValue<Vector2>();

            if (_moveInput.sqrMagnitude > 1f)
            {
                _moveInput.Normalize();
            }
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _moveInput * _moveSpeed;
        }

        private void OnDisable()
        {
            _moveAction?.Disable();
        }
    }
}
