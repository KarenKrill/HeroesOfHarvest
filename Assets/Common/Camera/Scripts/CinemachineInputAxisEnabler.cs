using UnityEngine;
using Unity.Cinemachine;

using Zenject;

using KarenKrill.UniCore.Input.Abstractions;
using UnityEngine.InputSystem;

namespace KarenKrill.UniCore.Utilities
{
    public enum CinemachineInputAxisEnablerType
    {
        ActionBased,
        ActionMapBased
    }
    [RequireComponent(typeof(CinemachineInputAxisController))]
    public class CinemachineInputAxisEnabler : MonoBehaviour
    {
        [Inject]
        public void Initialize(IBasicActionsProvider actionsProvider)
        {
            _actionsProvider = actionsProvider;
        }

        [SerializeField]
        private CinemachineInputAxisEnablerType _enablerType = CinemachineInputAxisEnablerType.ActionBased;
        [SerializeField]
        private ActionMap _axesActionMap = ActionMap.Player;
        [SerializeField]
        private InputActionReference _actionReference;

        private IBasicActionsProvider _actionsProvider;
        private CinemachineInputAxisController _cinemachineInputAxisController;

        private void Awake()
        {
            _cinemachineInputAxisController = GetComponent<CinemachineInputAxisController>();
        }
        private void OnEnable()
        {
            switch (_enablerType)
            {
                case CinemachineInputAxisEnablerType.ActionBased:
                    if (_actionReference != null)
                    {
                        _actionReference.action.performed += OnActionChanged;
                    }
                    break;
                case CinemachineInputAxisEnablerType.ActionMapBased:
                    _actionsProvider.ActionMapChanged += OnActionMapChanged;
                    break;
                default:
                    break;
            }
        }
        private void OnDisable()
        {
            switch (_enablerType)
            {
                case CinemachineInputAxisEnablerType.ActionBased:
                    if (_actionReference != null)
                    {
                        _actionReference.action.performed -= OnActionChanged;
                    }
                    break;
                case CinemachineInputAxisEnablerType.ActionMapBased:
                    _actionsProvider.ActionMapChanged -= OnActionMapChanged;
                    break;
                default:
                    break;
            }
        }

        private void OnActionMapChanged(ActionMap actionMap)
        {
            foreach (var controller in _cinemachineInputAxisController.Controllers)
            {
                controller.Enabled = actionMap == _axesActionMap;
            }
        }
        private void OnActionChanged(InputAction.CallbackContext ctx)
        {
            bool keyUp = ctx.action.WasReleasedThisFrame();
            foreach (var controller in _cinemachineInputAxisController.Controllers)
            {
                controller.Enabled = !keyUp;
            }
        }

    }
}
