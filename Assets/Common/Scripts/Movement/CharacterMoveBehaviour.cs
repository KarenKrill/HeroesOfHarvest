using UnityEngine;

using KarenKrill.Utilities;

namespace KarenKrill.Movement
{
    public class CharacterMoveBehaviour : MonoBehaviour
    {
        public float MaximumSpeed { get => _maximumSpeed; set => _maximumSpeed = value; }
        /// <summary>Range: [0..1]</summary>
        public float SpeedModifier { get => _speedModifier; set => _speedModifier = value; }
        /// <summary>Range: [0..1]</summary>
        public float GravityModifier { get => _gravityModifier; set => _gravityModifier = value; }
        public bool IsGrounded => _characterController.isGrounded;
        public bool IsGroundedRecently => _isGroundedRecently;
        public bool IsSliding => _isSliding;
        public bool IsFalling => _fallSpeed > 0;
        public bool IsPulsedUp => _isPulsedUp;

        public bool EnableCharController { get => _characterController.enabled; set => _characterController.enabled = value; }
        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
        public Vector2 LookDirection { get => _lookDirection; set => _lookDirection = value; }

        public void PulseUp(float distance, float gracePeriod, float inAirHorizontalSpeed)
        {
            _pulseUpDistance = distance;
            _pulseUpStartTime = Time.time;
            _pulseUpGracePeriod = gracePeriod;
            _inAirHorizontalSpeed = inAirHorizontalSpeed;
        }

        protected virtual void Awake()
        {
            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main.transform;
            }
            _characterControllerStepOffset = _characterController.stepOffset;
            _characterController.enabled = false;
        }
        protected virtual void OnEnable()
        {
            _characterController.enabled = true;
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
            UpdateMovement();
        }
        protected virtual void OnAnimatorMove()
        {
            if (_useRootMotion && _animator != null && _isGrounded && !_isSliding)
            {
                Vector3 velocity = _animator.deltaPosition;
                velocity.y = _fallSpeed * Time.deltaTime;
                _characterController.Move(velocity);
            }
        }

        [SerializeField]
        private CharacterController _characterController;
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private Animator _animator = null;
        [SerializeField]
        private float _maximumSpeed = 5f, _rotationDegreeSpeed = 360.0f;
        [SerializeField, Range(0, 1)]
        private float _speedModifier = 1f;
        [SerializeField, Range(0, 1)]
        private float _gravityModifier = 1f;
        [SerializeField]
        private float _gravityMultiplier = 1.5f;
        [SerializeField]
        private float _slidingDecelerationFactor = 3f;
        [SerializeField]
        private bool _useRootMotion = false;
        /// <summary>
        /// Use MoveDirection towards or LookDirection for character rotation
        /// </summary>
        [SerializeField]
        private bool _thirdPerson = false;

        private bool _isPulsedUp = false, _isSliding = false, _isGrounded = false;
        private bool _isGroundedRecently = false;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _lookDirection = Vector2.zero;
        private float _fallSpeed;
        private float _pulseUpGracePeriod = 0.2f;
        private float _pulseUpDistance = 2.0f, _inAirHorizontalSpeed = 3.0f;
        private Vector3 _slopeSlideVelocity;
        private float _characterControllerStepOffset;
        private float? _lastGroundedTime, _pulseUpStartTime;

        private void UpdateSlopeSlideVelocity()
        {
            if (Physics.Raycast(_characterController.transform.position, Vector3.down, out var hitInfo))
            {
                float slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
                if (slopeAngle >= _characterController.slopeLimit)
                {
                    _slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, _fallSpeed, 0), hitInfo.normal);
                    _isSliding = true;
                    return;
                }
            }
            if (_isSliding)
            {
                _slopeSlideVelocity -= _slidingDecelerationFactor * Time.deltaTime * _slopeSlideVelocity;
                if (_slopeSlideVelocity.magnitude > 1)
                {
                    return;
                }
            }
            _slopeSlideVelocity = Vector3.zero;
            _isSliding = false;
        }
        private void UpdateMovement()
        {
            float gravity = Physics.gravity.y * _gravityMultiplier * _gravityModifier;
            _fallSpeed += gravity * Time.deltaTime;

            UpdateSlopeSlideVelocity();

            _isGrounded = _characterController.isGrounded;
            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;
            }

            // Check on falling
            bool isFalling = !_isGrounded;
            if (Time.time - _lastGroundedTime <= _pulseUpGracePeriod) // grounded recently
            {
                _isGroundedRecently = true;
                _characterController.stepOffset = _characterControllerStepOffset;
                _isGrounded = true;
                isFalling = false;
                _isPulsedUp = false;
                if (!_isSliding)
                {
                    if (Time.time - _pulseUpStartTime <= _pulseUpGracePeriod) // pulsed up recently
                    {
                        _isPulsedUp = true;
                        _pulseUpStartTime = null;
                        _lastGroundedTime = null;
                        _fallSpeed = Mathf.Sqrt(_pulseUpDistance * -3 * gravity);
                    }
                    else
                    {
                        _fallSpeed = -0.5f; // to prevent isGrounded false positives
                    }
                }
            }
            else
            {
                _isGroundedRecently = false;
                _characterController.stepOffset = 0; // fix stuck in the wall while jumping
                if ((_isPulsedUp && _fallSpeed < 0) || _fallSpeed < -2)
                {
                    isFalling = true;
                }
            }

            // Direction & DirectionInputMagnitude usings
            var cameraRelativeQuaternion = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up);
            var direction = cameraRelativeQuaternion * _moveDirection;
            if (direction.magnitude > 1)
            {
                direction.Normalize();
            }
            var directionMagnitude = Mathf.Clamp(direction.magnitude, 0, SpeedModifier);
            if (!_useRootMotion)
            {
                float speed = directionMagnitude * _maximumSpeed;
                Vector3 velocity = speed * direction;
                velocity.y = _fallSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }
            if (_isSliding)
            {
                Vector3 velocity = _slopeSlideVelocity;
                velocity.y = _fallSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }
            else if (!_isGrounded) // jumping
            {
                float speed = directionMagnitude * _inAirHorizontalSpeed;
                Vector3 velocity = speed * direction;
                velocity.y = _fallSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }

            // Direction usings
            bool isMoving = direction != Vector3.zero;
            bool isLooking = _lookDirection != Vector3.zero;
            if (isMoving || isLooking)
            {
                if (_thirdPerson)
                {
                    var directionLookRotation = Quaternion.LookRotation(direction, Vector3.up);
                    var characterRotation = _characterController.transform.rotation;
                    var rotation = Quaternion.RotateTowards(characterRotation, directionLookRotation, _rotationDegreeSpeed * Time.deltaTime);
                    _characterController.transform.rotation = rotation;
                }
                else
                {
                    var rotation = Quaternion.RotateTowards(_characterController.transform.rotation, cameraRelativeQuaternion, 360);
                    _characterController.transform.rotation = rotation;
                }
            }

            if (_animator != null)
            {
                _animator.SetFloat("InputMagnitude", directionMagnitude, 0.5f, Time.deltaTime);
                _animator.SetBool("IsGrounded", _isGrounded);
                _animator.SetBool("IsJumping", _isPulsedUp);
                _animator.SetBool("IsFalling", isFalling);
                _animator.SetBool("IsMoving", isMoving);
                _animator.SetBool("IsLooking", isLooking);
            }
        }
    }
}
