using UnityEngine;
using Unity.Cinemachine;

using Zenject;

using KarenKrill.Input.Abstractions;

namespace KarenKrill.Utilities
{

    [RequireComponent(typeof(CinemachineInputAxisController))]
    public class CinemachineInputAxisEnabler : MonoBehaviour
    {
        [Inject]
        public void Initialize(IBasicActionsProvider actionsProvider)
        {
            _actionsProvider = actionsProvider;
        }

        [SerializeField]
        private ActionMap _axesActionMap = ActionMap.Player;

        private IBasicActionsProvider _actionsProvider;
        private CinemachineInputAxisController _cinemachineInputAxisController;

        private void Awake()
        {
            _cinemachineInputAxisController = GetComponent<CinemachineInputAxisController>();
        }
        private void OnEnable()
        {
            _actionsProvider.ActionMapChanged += OnActionMapChanged;
        }
        private void OnDisable()
        {
            _actionsProvider.ActionMapChanged -= OnActionMapChanged;
        }

        private void OnActionMapChanged(ActionMap actionMap)
        {
            foreach (var controller in _cinemachineInputAxisController.Controllers)
            {
                controller.Enabled = actionMap == _axesActionMap;
            }
        }
    }
}
