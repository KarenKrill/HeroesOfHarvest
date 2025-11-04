using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace HeroesOfHarvest.Movement
{
    public class NavMeshMoveBehaviour : MonoBehaviour
    {
        public event Action PathCancelled;
        public event Action PathCompleted;

        public float CompletionMonitorPeriodTime { get; set; } = 0.1f;

        public bool TryGoTo(Vector3 target)
        {
            _isExternalTarget = true;
            if (!TryGoTo(target, true))
            {
                _isExternalTarget = false;
                return false;
            }
            return true;
        }

        protected virtual bool IsDestinationValid(Vector3 destination) => true;
        protected virtual void OnPathCanceled()
        {
            _isExternalTarget = false;
            StopCoroutine(_completionMonitorCoroutine);
            _animator.SetFloat("InputMagnitude", 0);
            _completionMonitorCoroutine = null;
            PathCancelled?.Invoke();
        }
        protected virtual void OnPathCompleted()
        {
            PathCompleted?.Invoke();
        }

        [SerializeField]
        private InputActionReference _startMoveAction;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private LayerMask _walkableLayers;
        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float _maxTargetDetectDistance = 100;
        [SerializeField]
        private float _facingToTargetSpeed = 5f;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[5];
        private Coroutine _completionMonitorCoroutine = null;
        private bool _isExternalTarget = false;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            Debug.Assert(_camera != null);
        }
        private void OnEnable()
        {
            _startMoveAction.action.performed += OnStartMoveActionPerformed;
        }
        private void OnDisable()
        {
            _startMoveAction.action.performed -= OnStartMoveActionPerformed;
        }

        private void OnStartMoveActionPerformed(InputAction.CallbackContext ctx)
        {
            var pointerPosition = Pointer.current.position.ReadValue();
            var mouseRay = _camera.ScreenPointToRay(pointerPosition);
            if (TryGetNearestRaycastHit(mouseRay, out var raycastHit))
            {
                if (IsWalkableRaycastHit(raycastHit.Value))
                {
                    var destination = raycastHit.Value.point;
                    if (IsDestinationValid(destination))
                    {
                        _ = TryGoTo(destination, false);
                    }
                }
            }
        }
        private bool TryGoTo(Vector3 destination, bool ignoreUnreachable)
        {
            var navMeshPath = new NavMeshPath();
            if (_navMeshAgent.CalculatePath(destination, navMeshPath))
            {
                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    var lastCorner = navMeshPath.corners[^1];
                    if (ignoreUnreachable || Mathf.Abs(lastCorner.x - destination.x) <= float.Epsilon && Mathf.Abs(lastCorner.z - destination.z) <= float.Epsilon)
                    {
                        _navMeshAgent.path = navMeshPath;
                        var isExternalTargetCall = ignoreUnreachable; // dirty hack to distinguish between external and internal calls
                        if (_isExternalTarget && !isExternalTargetCall && _completionMonitorCoroutine != null)
                        {
                            OnPathCanceled();
                        }
                        else _completionMonitorCoroutine ??= StartCoroutine(PathCompletionMonitorCoroutine());
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"Partial path cancelled (destination - {destination}; last reachable point - {lastCorner}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Path to {destination} not found");
            }
            return false;
        }
        private bool TryGetNearestRaycastHit(Ray ray, [NotNullWhen(true)] out RaycastHit? hit)
        {
            int hitsCount;
            if ((hitsCount = Physics.RaycastNonAlloc(ray, _raycastHits, _maxTargetDetectDistance)) > 0)
            {
                int minIndex = 0;
                var minDistance = Vector3.Distance(ray.origin, _raycastHits[0].point);
                for (int i = 1; i < hitsCount; i++)
                {
                    var distance = Vector3.Distance(ray.origin, _raycastHits[i].point);
                    if (distance < minDistance)
                    {
                        minIndex = i;
                        minDistance = distance;
                    }
                }
                hit = _raycastHits[minIndex];
                return true;
            }
            else
            {
                hit = null;
                return false;
            }
        }
        private bool IsWalkableRaycastHit(in RaycastHit raycastHit)
        {
            return (_walkableLayers.value & (1 << raycastHit.collider.gameObject.layer)) != 0;
        }
        private bool IsDestinationReached()
        { //  Check if the model is moving on the NavMesh
            if (_navMeshAgent.pathPending)
            { //  because takes some time to update the remainingDistance and will return a wrong value
                return Vector3.Distance(_navMeshAgent.transform.position, _navMeshAgent.pathEndPosition) <= _navMeshAgent.stoppingDistance;
            }
            else
            {
                return (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance);
            }
        }
        private IEnumerator PathCompletionMonitorCoroutine()
        {
            while (!IsDestinationReached())
            {
                FaceToTarget();
                _animator.SetFloat("InputMagnitude", _navMeshAgent.velocity.magnitude);
                yield return null;
            }
            _animator.SetFloat("InputMagnitude", 0);
            if (_isExternalTarget)
            {
                OnPathCompleted();
            }
            _completionMonitorCoroutine = null;
        }
        void FaceToTarget()
        {
            var navMeshTransform = _navMeshAgent.transform;
            var forwardDirection = (_navMeshAgent.destination - navMeshTransform.position).normalized;
            forwardDirection.y = 0;
            if (forwardDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(forwardDirection);
                navMeshTransform.rotation = Quaternion.Slerp(navMeshTransform.rotation, targetRotation, Time.deltaTime * _facingToTargetSpeed);
            }
        }
    }
}
