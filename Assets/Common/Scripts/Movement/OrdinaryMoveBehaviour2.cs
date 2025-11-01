using System;

using UnityEngine;

using Zenject;

using KarenKrill.Utilities;

using KarenKrill.Input.Abstractions;

namespace KarenKrill.Movement
{
    public enum MoveState
    {
        Grounded,
        Jumping,
        Falling,
        Sliding,
        /// <remarks>
        /// Not supported yet
        /// </remarks>
        Climbing,
        /// <remarks>
        /// Not supported yet
        /// </remarks>
        Flying
    }
    public enum CameraType
    {
        FirstPerson,
        ThirdPerson
    }

    public class OrdinaryMoveBehaviour2 : MonoBehaviour
    {
        [Inject]
        public void Initialize(IBasicPlayerActionsProvider playerActionsProvider)
        {
            _playerActionsProvider = playerActionsProvider;
        }

        protected virtual void Awake()
        {
            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }
        protected virtual void OnEnable()
        {
            _characterController.enabled = true;
            _playerActionsProvider.MoveStarted += OnMoveStarted;
            _playerActionsProvider.MoveCancel += OnMoveCanceled;
            _playerActionsProvider.Look += OnLookChanged;
            _playerActionsProvider.Jump += OnJumpPerformed;
            _playerActionsProvider.JumpCancel += OnJumpCanceled;
        }
        protected virtual void OnDisable()
        {
            if (!_characterController.IsNullOrDestroyed())
            {
                _characterController.enabled = false;
            }
        }
        protected virtual void Update()
        {
            if (_movingInputStarted)
            {
                UpdateMovement();
            }
            if (!_characterController.isGrounded)
            {
                float gravity = Physics.gravity.y * _gravityMultiplier * _gravityModifier;
                var fallSpeed = _motionVelocity.Y + gravity * Time.deltaTime;
                if (Mathf.Abs(fallSpeed) > _maxFallSpeedModule)
                {
                    fallSpeed = -_maxFallSpeedModule;
                }
                _motionVelocity.Y = fallSpeed;
            }
            if(_characterController.isGrounded)
            {
                if (_jumpingInputStarting)
                {
                    float gravity = Mathf.Abs(Physics.gravity.y) * _gravityMultiplier * _gravityModifier;
                    var fallSpeed = Mathf.Sqrt(2 * gravity * _maxJumpHeight);
                    _motionVelocity.Y = fallSpeed;
                }
                else
                {
                    _motionVelocity.Y = -0.5f; // to prevent isGrounded false positives
                }
            }


            //if (_movingInputStarted || _motionVelocity.IsDirty)
            {
                //_motionVelocity.ResetDirtyFlag();
                _characterController.Move(_motionVelocity.Vector * Time.deltaTime);
            }
            if(_movingInputStarted || _isLookChanged)
            {
                if (_movingInputStarted && _cameraType == CameraType.ThirdPerson)
                {
                    var moveInputDelta = _playerActionsProvider.LastMoveDelta;
                    var moveDirection = new Vector3(moveInputDelta.x, 0, moveInputDelta.y);
                    var direction = _cameraRelativeRotation * moveDirection; // can't be zero while _movingInputStarted == true
                    var directionLookRotation = Quaternion.LookRotation(direction, Vector3.up);
                    var characterRotation = _characterController.transform.rotation;
                    var rotation = Quaternion.RotateTowards(characterRotation, directionLookRotation, _maxAngularSpeedInDegrees * Time.deltaTime);
                    _characterController.transform.rotation = rotation;
                }
                else if(_isLookChanged && _cameraType == CameraType.FirstPerson)
                {
                    var characterRotation = _characterController.transform.rotation;
                    var rotation = Quaternion.RotateTowards(characterRotation, _cameraRelativeRotation, _maxAngularSpeedInDegrees * Time.deltaTime);
                    _characterController.transform.rotation = rotation;
                }
            }
        }

        [SerializeField]
        private CharacterController _characterController;
        [SerializeField]
        private Transform _cameraTransform;
        /// <summary>
        /// Used to rotate character in look direction
        /// </summary>
        /// <remarks>In firs-person look direction is <see cref="_cameraTransform"/> forward, </remarks>
        [SerializeField]
        private CameraType _cameraType = CameraType.FirstPerson;
        [SerializeField]
        private float _maxSpeed = 1, _maxInAirSpeed = 0.5f;
        [SerializeField]
        private float _maxAngularSpeedInDegrees = 360.0f;
        [SerializeField, Range(0, 1)]
        private float _speedModifier = 1f;

        [SerializeField, Min(0)]
        private float _maxJumpHeight = 2;
        [SerializeField, Range(0, 1)]
        private float _gravityModifier = 1f;
        [SerializeField, Min(1)]
        private float _gravityMultiplier = 1.5f;
        [SerializeField, Min(0)]
        private float _maxFallSpeedModule = 100;

        private IBasicPlayerActionsProvider _playerActionsProvider;
        private bool _movingInputStarted = false;
        private bool _jumpingInputStarting = false;
        private bool _isLookChanged = false;

        private Quaternion _cameraRelativeRotation;
        private readonly DirtyVector3 _motionVelocity = new();

        private void OnMoveStarted()
        {
            _movingInputStarted = true;
        }
        private void OnMoveCanceled()
        {
            _movingInputStarted = false;
            _motionVelocity.X = 0;
            _motionVelocity.Z = 0;
        }
        private void OnLookChanged(Vector2 lookDelta)
        {
            var yawAngle = _cameraTransform.rotation.eulerAngles.y;
            _cameraRelativeRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
            _isLookChanged = true;
        }
        private void OnJumpPerformed()
        {
            _jumpingInputStarting = true;
        }
        private void OnJumpCanceled()
        {
            _jumpingInputStarting = false;
        }
        private void UpdateMovement()
        {
            var moveInputDelta = _playerActionsProvider.LastMoveDelta;
            var moveDirection = new Vector3(moveInputDelta.x, 0, moveInputDelta.y);
            var direction = _cameraRelativeRotation * moveDirection;
            var directionMagnitude = Mathf.Clamp(direction.magnitude, 0, _speedModifier);
            float speed = directionMagnitude * (_characterController.isGrounded ? _maxSpeed : _maxInAirSpeed);
            var velocity = speed * direction;
            _motionVelocity.X = velocity.x;
            _motionVelocity.Z = velocity.z;
        }

        private class DirtyVector3
        {
            public float X
            {
                get => _vector.x;
                set
                {
                    if (FloatEpsilonNotEquals(_vector.x, value))
                    {
                        _vector.x = value;
                        _isDirty = true;
                    }
                }
            }
            public float Y
            {
                get => _vector.y;
                set
                {
                    if (FloatEpsilonNotEquals(_vector.y, value))
                    {
                        _vector.y = value;
                        _isDirty = true;
                    }
                }
            }
            public float Z
            {
                get => _vector.z;
                set
                {
                    if (FloatEpsilonNotEquals(_vector.z, value))
                    {
                        _vector.z = value;
                        _isDirty = true;
                    }
                }
            }
            public Vector3 Vector => _vector;
            public bool IsDirty => _isDirty;

            public DirtyVector3()
            {
                _vector = Vector3.zero;
                _isDirty = false;
            }
            public DirtyVector3(Vector3 vector, bool isInitialDirty = true)
            {
                _vector = vector;
                _isDirty = isInitialDirty;
            }
            public bool ResetDirtyFlag() => _isDirty = false;

            private Vector3 _vector;
            private bool _isDirty;

            private bool FloatEpsilonNotEquals(float a, float b) => Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b)) > Mathf.Epsilon;
        }
    }
}
