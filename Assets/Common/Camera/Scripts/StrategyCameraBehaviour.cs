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
        }
        private void OnDisable()
        {
            _zoomActionReference.action.performed -= OnZoomActionPerformed;
            _moveActionReference.action.performed -= OnMoveActionPerformed;
            _moveActionReference.action.canceled -= OnMoveActionCanceled;
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
                if (_zoomCoroutine != null)
                {
                    StopCoroutine(_zoomCoroutine);
                }
                var zoomValue = _positionComposer.CameraDistance - zoomVector.y * _zoomGain;
                var cameraDistance = Mathf.Clamp(zoomValue, _zoomMin, _zoomMax);
                _zoomCoroutine = StartCoroutine(SmoothZoomCoroutine(cameraDistance));
            }
        }
        private void OnMoveActionPerformed(InputAction.CallbackContext ctx)
        {
            var moveVector = ctx.ReadValue<Vector2>();
            _moveDirection = new Vector3(moveVector.x, 0, moveVector.y);
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
        private void OnMoveActionCanceled(InputAction.CallbackContext obj)
        {
            _moveCts?.Cancel();
            _moveCoroutine = null;
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
        private IEnumerator TargetMoveCoroutine(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var speedFactor = _targetSpeedCurve.Evaluate(_positionComposer.CameraDistance / _zoomMax);
                if (_zoomCoroutine != null)
                { // Optimization to avoid recalculating the camera-relative direction every frame
                    var yawAngle = _mainCameraTransform.rotation.eulerAngles.y;
                    var cameraRelativeRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
                    _camRelativeMoveDirection = cameraRelativeRotation * _moveDirection;
                }
                _cameraTarget.transform.Translate(_targetSpeed * speedFactor * Time.deltaTime * _camRelativeMoveDirection);
                yield return null;
            }
        }
    }
}
