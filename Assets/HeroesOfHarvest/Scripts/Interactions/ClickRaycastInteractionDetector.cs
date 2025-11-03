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
            _actionsProvider.InteractClickUp += OnInteract;
        }
        protected override void InputUnsubscribe()
        {
            _actionsProvider.CameraMove -= OnLookOrMove;
            _actionsProvider.InteractPoint -= OnLookOrMove;
            _actionsProvider.InteractClickUp -= OnInteract;
        }

        [SerializeField]
        private InteractorBase _interactor;
        [SerializeField]
        private Camera _camera;

        private IStrategyGameActionsProvider _actionsProvider;

        private void OnLookOrMove(Vector2 delta)
        {
            var ray = _camera.ScreenPointToRay(_actionsProvider.LastPointDelta);
            OnLookChanged(_interactor, ray);
        }
        private void OnInteract() => OnInteract(_interactor);
    }
}
