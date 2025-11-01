using UnityEngine;

using Zenject;

using KarenKrill.Input.Abstractions;

namespace KarenKrill.Movement
{
    public class InputCharacterMoveContoller : CharacterMoveBehaviour
    {
        [Inject]
        public void Initialize(IBasicPlayerActionsProvider playerActionsProvider)
        {
            _playerActionsProvider = playerActionsProvider;
        }

        protected override void Awake()
        {
            base.Awake();
            SpeedModifier = 0.5f;
            GravityModifier = 0.5f;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _playerActionsProvider.Sprint += OnRun;
            _playerActionsProvider.SprintCancel += OnRunCancel;
            _playerActionsProvider.Jump += OnJump;
            _playerActionsProvider.JumpCancel += OnJumpCancel;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            _playerActionsProvider.Sprint -= OnRun;
            _playerActionsProvider.SprintCancel -= OnRunCancel;
            _playerActionsProvider.Jump -= OnJump;
            _playerActionsProvider.JumpCancel -= OnJumpCancel;
        }
        protected override void Update()
        {
            base.Update();
            MoveDirection = new Vector3(_playerActionsProvider.LastMoveDelta.x, 0, _playerActionsProvider.LastMoveDelta.y);
            LookDirection = _playerActionsProvider.LastLookDelta;
            if (IsPulsedUp)
            {
                if (!_isJumpPressed && IsFalling) // если короткое нажатие
                {
                    GravityModifier = 1f; // ускоряем прыжок
                }
                else
                {
                    GravityModifier = 0.5f;
                }
            }
        }

        [SerializeField]
        private float _jumpHeight = 2.0f, _jumpHorizontalSpeed = 3.0f;
        [SerializeField]
        private float _jumpButtonGracePeriod = 0.2f;

        private IBasicPlayerActionsProvider _playerActionsProvider;
        private bool _isJumpPressed = false;

        private void OnRun()
        {
            SpeedModifier = 1f;
        }
        private void OnRunCancel()
        {
            SpeedModifier = 0.5f;
        }
        private void OnJump()
        {
            PulseUp(_jumpHeight, _jumpButtonGracePeriod, _jumpHorizontalSpeed);
            _isJumpPressed = true;
        }
        private void OnJumpCancel()
        {
            _isJumpPressed = false;
        }
    }
}