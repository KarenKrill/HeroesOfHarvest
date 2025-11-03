using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace HeroesOfHarvest
{
    public class NavMeshMoveBehaviour : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _startMoveAction;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private LayerMask _walkableLayers;
        [SerializeField]
        private NavMeshAgent _navMeshAgent;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[1];

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
            if (Physics.RaycastNonAlloc(mouseRay, _raycastHits, 100, _walkableLayers) == 1)
            {
                _navMeshAgent.destination = _raycastHits[0].point;
            }
        }
    }
}
