using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KarenKrill.Cinematics
{
    public class StrategyCameraBehaviour : MonoBehaviour
    {
        [SerializeField]
        private CinemachinePositionComposer _positionComposer;
        [SerializeField]
        private CinemachineInputAxisController _inputAxisController;
        [SerializeField]
        private Transform _cameraTarget;
        [SerializeField]
        private Transform _mainCameraTransform;
        [SerializeField]
        private float _targetSpeed = 5;
        [SerializeField]
        private AnimationCurve _targetSpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private float _swipeDetectionTime = .3f;
        [SerializeField]
        private float _swipePositionTracehold = .001f;
        [Header("Zoom range")]
        [SerializeField]
        private float _zoomMin = 10;
        [SerializeField]
        private float _zoomMax = 20;
        [Header("Input")]
        [SerializeField]
        private InputActionReference _zoomActionReference;
        [SerializeField]
        private InputActionReference _moveActionReference;
        [SerializeField]
        private InputActionReference _swipePressActionReference;
        [SerializeField]
        private InputActionReference _swipePointActionReference;
        [Header("Input gain")]
        [SerializeField]
        private float _zoomGain = 5;
        [SerializeField]
        private float _panGain = 5;
        [SerializeField]
        private float _tiltGain = -5;

        private Coroutine _zoomCoroutine = null;
        private Coroutine _moveCoroutine = null;
        private CancellationTokenSource _moveCts;
        private Vector3 _moveDirection;
        private Vector3 _camRelativeMoveDirection;
        private Vector2 _previousSwipePointPosition;
        private double _swipePressStartTime = double.MaxValue;
        private bool _isSwipeStarted = false;
        private bool _isSwipeKeyPressed = false;
        private bool _isMoveKeyPressed = false;
        private readonly InputAction _touch0PressAction = new(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        private readonly InputAction _touch1PressAction = new(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
        private readonly InputAction _touch0PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        private readonly InputAction _touch1PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
        private float _prevTouchesDistance = 0;
        private int _touchesCount = 0;

        private void Awake()
        {
            if (_mainCameraTransform == null && Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }
            Debug.Assert(_mainCameraTransform != null);
        }
        private void OnEnable()
        {
            _zoomActionReference.action.performed += OnZoomActionPerformed;
            _moveActionReference.action.performed += OnMoveActionPerformed;
            _moveActionReference.action.canceled += OnMoveActionCanceled;
            _swipePressActionReference.action.performed += OnSwipeActionPerformed;
            _swipePointActionReference.action.performed += OnSwipePointActionPerformed;
            _touch0PressAction.performed += OnTouchPressActionPerformed;
            _touch1PressAction.performed += OnTouchPressActionPerformed;
            _touch0PressAction.canceled += OnTouchPressActionCanceled;
            _touch1PressAction.canceled += OnTouchPressActionCanceled;
            _touch0PosAction.performed += OnTouchPosActionPerformed;
            _touch1PosAction.performed += OnTouchPosActionPerformed;
            _touch0PressAction.Enable();
            _touch1PressAction.Enable();
            _touch0PosAction.Enable();
            _touch1PosAction.Enable();
        }
        private void OnDisable()
        {
            _touch0PressAction.Disable();
            _touch1PressAction.Disable();
            _touch0PosAction.Disable();
            _touch1PosAction.Disable();
            _zoomActionReference.action.performed -= OnZoomActionPerformed;
            _moveActionReference.action.performed -= OnMoveActionPerformed;
            _moveActionReference.action.canceled -= OnMoveActionCanceled;
            _swipePressActionReference.action.performed -= OnSwipeActionPerformed;
            _swipePointActionReference.action.performed -= OnSwipePointActionPerformed;
            _touch0PressAction.performed -= OnTouchPressActionPerformed;
            _touch1PressAction.performed -= OnTouchPressActionPerformed;
            _touch0PressAction.canceled -= OnTouchPressActionCanceled;
            _touch1PressAction.canceled -= OnTouchPressActionCanceled;
            _touch0PosAction.performed -= OnTouchPosActionPerformed;
            _touch1PosAction.performed -= OnTouchPosActionPerformed;
        }
        private void OnValidate()
        {
            Debug.Assert(_inputAxisController != null);
            Debug.Assert(_inputAxisController.Controllers.Count == 2);
            Debug.Assert(_inputAxisController.Controllers[0].Name == "Look X (Pan)");
            Debug.Assert(_inputAxisController.Controllers[1].Name == "Look Y (Tilt)");
            _inputAxisController.Controllers[0].Input.Gain = _panGain;
            _inputAxisController.Controllers[1].Input.Gain = _tiltGain;
            if (_zoomMin > _zoomMax)
            {
                _zoomMin = _zoomMax;
            }
        }

        private void OnZoomActionPerformed(InputAction.CallbackContext ctx)
        {
            var zoomVector = ctx.ReadValue<Vector2>();
            if (zoomVector.y != 0)
            {
                UpdateCameraZoom(zoomVector.y);
            }
        }
        private void OnMoveActionPerformed(InputAction.CallbackContext ctx)
        {
            var moveDirection = ctx.ReadValue<Vector2>();
            _isMoveKeyPressed = true;
            UpdateCameraMove(moveDirection);
        }
        private void OnMoveActionCanceled(InputAction.CallbackContext ctx)
        {
            _isMoveKeyPressed = false;
            OnMoveSourceStopped();
        }
        private void OnSwipeActionPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.action.WasPressedThisFrame())
            {
                _isSwipeKeyPressed = true;
                _previousSwipePointPosition = _swipePointActionReference.action.ReadValue<Vector2>();
                _swipePressStartTime = ctx.startTime;
            }
            else if (_isSwipeKeyPressed && ctx.action.WasReleasedThisFrame())
            {
                _isSwipeKeyPressed = _isSwipeStarted = false;
                OnMoveSourceStopped();
            }
        }
        private void OnSwipePointActionPerformed(InputAction.CallbackContext ctx)
        {
            if (_isSwipeKeyPressed)
            {
                if (_touchesCount <= 1 && !_isSwipeStarted)
                {
                    var holdTime = ctx.startTime - _swipePressStartTime;
                    if (holdTime >= _swipeDetectionTime)
                    {
                        _isSwipeStarted = true;
                    }
                }
                if (_isSwipeStarted)
                {
                    if (_touchesCount > 1)
                    {
                        _isSwipeKeyPressed = _isSwipeStarted = false;
                        OnMoveSourceStopped();
                        return;
                    }
                    var swipePointPosition = ctx.action.ReadValue<Vector2>();
                    var swipeDirection = (_previousSwipePointPosition - swipePointPosition);
                    swipeDirection.Normalize();
                    _previousSwipePointPosition = swipePointPosition;
                    bool isXSwipeDirectionTooSmall = Mathf.Abs(swipeDirection.x) <= _swipePositionTracehold;
                    bool isYSwipeDirectionTooSmall = Mathf.Abs(swipeDirection.y) <= _swipePositionTracehold;
                    if (!isXSwipeDirectionTooSmall || !isYSwipeDirectionTooSmall)
                    {
                        if (isXSwipeDirectionTooSmall)
                        {
                            swipeDirection.x = 0;
                        }
                        else if (isYSwipeDirectionTooSmall)
                        {
                            swipeDirection.y = 0;
                        }
                        UpdateCameraMove(swipeDirection);
                    }
                }
            }
        }
        private void OnTouchPressActionPerformed(InputAction.CallbackContext ctx)
        {
            _touchesCount++;
        }
        private void OnTouchPressActionCanceled(InputAction.CallbackContext ctx)
        {
            _touchesCount--;
            _prevTouchesDistance = 0;
        }
        private void OnTouchPosActionPerformed(InputAction.CallbackContext ctx)
        {
            if (_touchesCount > 1)
            {
                var distance = (_touch0PosAction.ReadValue<Vector2>() - _touch1PosAction.ReadValue<Vector2>()).magnitude;
                var diff = _prevTouchesDistance == 0 ? 0 : distance - _prevTouchesDistance;
                _prevTouchesDistance = distance;
                UpdateCameraZoom(diff);
            }
        }

        private void UpdateCameraMove(Vector2 normalizedMoveDirection)
        {
            _moveDirection = new Vector3(normalizedMoveDirection.x, 0, normalizedMoveDirection.y);
            var yawAngle = _mainCameraTransform.rotation.eulerAngles.y;
            var cameraRelativeRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
            _camRelativeMoveDirection = cameraRelativeRotation * _moveDirection;
            if (_moveCoroutine == null)
            {
                _moveCts?.Dispose();
                _moveCts = new();
                _moveCoroutine = StartCoroutine(TargetMoveCoroutine(_moveCts.Token));
            }
        }
        private void UpdateCameraZoom(float zoomValue2)
        {
            if (_zoomCoroutine != null)
            {
                StopCoroutine(_zoomCoroutine);
            }
            var zoomValue = _positionComposer.CameraDistance - zoomValue2 * _zoomGain;
            var cameraDistance = Mathf.Clamp(zoomValue, _zoomMin, _zoomMax);
            _zoomCoroutine = StartCoroutine(SmoothZoomCoroutine(cameraDistance));
        }
        private void OnMoveSourceStopped()
        {
            if (!_isSwipeStarted && !_isMoveKeyPressed)
            {
                _moveCts?.Cancel();
                _moveCoroutine = null;
            }
        }
        private float CalcDesiredSpeed()
        {
            var speedFactor = _targetSpeedCurve.Evaluate(_positionComposer.CameraDistance / _zoomMax);
            return _targetSpeed * speedFactor;
        }
        private IEnumerator TargetMoveCoroutine(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var speed = CalcDesiredSpeed();
                if (_zoomCoroutine != null)
                { // Optimization to avoid recalculating the camera-relative direction every frame
                    var yawAngle = _mainCameraTransform.rotation.eulerAngles.y;
                    var cameraRelativeRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
                    _camRelativeMoveDirection = cameraRelativeRotation * _moveDirection;
                }
                _cameraTarget.transform.Translate(speed * Time.deltaTime * _camRelativeMoveDirection);
                yield return null;
            }
        }
        private IEnumerator SmoothZoomCoroutine(float targetDistance)
        {
            while (Mathf.Abs(_positionComposer.CameraDistance - targetDistance) > float.Epsilon)
            {
                _positionComposer.CameraDistance = Mathf.Lerp(_positionComposer.CameraDistance, targetDistance, Time.deltaTime * _zoomGain);
                yield return null;
            }
            _zoomCoroutine = null;
        }
    }
}
