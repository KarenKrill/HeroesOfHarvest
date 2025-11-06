using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions;
using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Input.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    public class ClickRaycastInteractionDetector : RaycastInteractionDetectorBase
    {
        [Inject]
        public void Initialize(IStrategyGameActionsProvider playerActionsProvider, IInteractionTargetRegistry interactionTargetRegistry)
        {
            _actionsProvider = playerActionsProvider;
            base.Initialize(interactionTargetRegistry);
        }

        protected override void InputSubscribe()
        {
            _actionsProvider.CameraMove += OnLookOrMove;
            _actionsProvider.InteractPoint += OnLookOrMove;
            _actionsProvider.InteractClickDown += OnInteractClickDown;
            _actionsProvider.InteractClickUp += OnInteractClickUp;
        }
        protected override void InputUnsubscribe()
        {
            _actionsProvider.CameraMove -= OnLookOrMove;
            _actionsProvider.InteractPoint -= OnLookOrMove;
            _actionsProvider.InteractClickUp -= OnInteractClickUp;
        }

        [SerializeField]
        private InteractorBase _interactor;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _maxClickHoldTime = .3f;

        private IStrategyGameActionsProvider _actionsProvider;
        private float _startClickTime = 0;

        private void OnLookOrMove(Vector2 delta)
        {
            var ray = _camera.ScreenPointToRay(_actionsProvider.LastPointDelta);
            OnLookChanged(_interactor, ray);
        }
        private void OnInteractClickDown()
        {
            _startClickTime = Time.time;
        }
        private void OnInteractClickUp()
        {
            var holdTime = Time.time - _startClickTime;
            if (holdTime < _maxClickHoldTime)
            {
                OnInteract(_interactor);
            }
        }
    }
}
