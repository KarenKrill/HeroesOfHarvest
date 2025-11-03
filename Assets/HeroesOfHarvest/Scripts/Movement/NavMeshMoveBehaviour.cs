using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace HeroesOfHarvest
{
    public class NavMeshMoveBehaviour : MonoBehaviour, IUnitMover
    {
        public event Action PathCancelled;
        public event Action PathCompleted;

        public float CompletionMonitorPeriodTime { get; set; } = 0.3f;

        public void GoTo(Vector3 target)
        {
            _isExternalTarget = true;
            if (!TryGoTo(target, true))
            {
                _isExternalTarget = false;
            }
        }

        private bool _isExternalTarget = false;

        [SerializeField]
        private InputActionReference _startMoveAction;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private LayerMask _walkableLayers;
        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private float _maxTargetDetectDistance = 100;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[5];
        private Coroutine _completionMonitorCoroutine = null;

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
                    _ = TryGoTo(raycastHit.Value.point);
                }
            }
        }
        private void OnPathCanceled()
        {
            _isExternalTarget = false;
            StopCoroutine(_completionMonitorCoroutine);
            _completionMonitorCoroutine = null;
            PathCancelled?.Invoke();
        }
        private void OnPathCompleted()
        {
            PathCompleted?.Invoke();
        }

        private bool TryGoTo(Vector3 destination, bool ignoreUnreachable = false)
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
                        if (_isExternalTarget)
                        {
                            if (_completionMonitorCoroutine != null)
                            {
                                OnPathCanceled();
                            }
                            else
                            {
                                _completionMonitorCoroutine = StartCoroutine(PathCompletionMonitorCoroutine());
                            }
                            return true;
                        }
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
                yield return new WaitForSeconds(CompletionMonitorPeriodTime);
            }
            if (_isExternalTarget)
            {
                OnPathCompleted();
            }
            _completionMonitorCoroutine = null;
        }
    }
}
