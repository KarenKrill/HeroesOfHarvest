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
            var mousePosition = Mouse.current.position.ReadValue();
            var mouseRay = _camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(mouseRay, out var hit, 100, _walkableLayers))
            {
                _navMeshAgent.destination = hit.point;
            }
        }
    }
}
