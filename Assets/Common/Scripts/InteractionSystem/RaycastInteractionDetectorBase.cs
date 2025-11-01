using UnityEngine;

using Zenject;

namespace KarenKrill.Interactions
{
    using Abstractions;

    public abstract class RaycastInteractionDetectorBase : MonoBehaviour
    {
        [Inject]
        public void Initialize(IInteractionTargetRegistry interactionTargetRegistry)
        {
            _interactionTargetRegistry = interactionTargetRegistry;
        }

        protected abstract void InputSubscribe();
        protected abstract void InputUnsubscribe();
        protected virtual void OnEnable()
        {
            InputSubscribe();
            _interactionTargetRegistry.Unregistred += OnInteractionTargetUnregistred;
        }
        protected virtual void OnDisable()
        {
            InputUnsubscribe();
            _interactionTargetRegistry.Unregistred -= OnInteractionTargetUnregistred;
        }
        protected void OnLookChanged(IInteractor interactor, Ray ray)
        {
            var hitsCount = Physics.RaycastNonAlloc(ray, _cachedRaycastHits, _detectDistance, InteractableLayer);
            IInteractionTarget nearestTarget = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < hitsCount; i++)
            {
                if (minDistance > _cachedRaycastHits[i].distance && _cachedRaycastHits[i].collider.TryGetComponent<IInteractionTarget>(out var interactionTarget))
                {
                    if (_interactionTargetRegistry.Contains(interactionTarget))
                    {
                        nearestTarget = interactionTarget;
                        minDistance = _cachedRaycastHits[i].distance;
                    }
                }
            }
            if (nearestTarget != null)
            {
                OnTargetFocused(nearestTarget, interactor);
            }
            else
            {
                OnTargetLostFocus();
            }
        }
        protected void OnInteract(IInteractor interactor)
        {
            bool isInteractionAllowed = _lastAvailableInteractionTarget?.Interactable.Interact(interactor) ?? true;
            if (isInteractionAllowed)
            {
                interactor?.Interact(_lastAvailableInteractionTarget?.Interactable);
            }
        }

        [SerializeField]
        private float _detectDistance = 1;
        [SerializeField]
        private LayerMask InteractableLayer;

        private IInteractionTargetRegistry _interactionTargetRegistry;
        private IInteractionTarget _lastAvailableInteractionTarget = null;
        private IInteractor _lastInteractor = null;
        private const int CACHED_RAYCAST_HITS_COUNT = 5;
        private readonly RaycastHit[] _cachedRaycastHits = new RaycastHit[CACHED_RAYCAST_HITS_COUNT];

        private void OnTargetFocused(IInteractionTarget interactionTarget, IInteractor interactor)
        {
            _lastAvailableInteractionTarget?.Interactable.SetInteractionAvailability(_lastInteractor, false);
            _lastInteractor?.SetInteractionAvailability(_lastAvailableInteractionTarget?.Interactable, false);
            _lastAvailableInteractionTarget = interactionTarget;
            _lastInteractor = interactor;
            interactionTarget.Interactable.SetInteractionAvailability(interactor, true);
            interactor.SetInteractionAvailability(_lastAvailableInteractionTarget?.Interactable, true);
        }
        private void OnTargetLostFocus()
        {
            // target not found:
            _lastAvailableInteractionTarget?.Interactable.SetInteractionAvailability(_lastInteractor, false);
            _lastInteractor?.SetInteractionAvailability(_lastAvailableInteractionTarget?.Interactable, false);
            _lastAvailableInteractionTarget = null;
            _lastInteractor = null;
        }
        private void OnInteractionTargetUnregistred(IInteractionTarget target)
        {
            if (_lastAvailableInteractionTarget == target)
            {
                OnTargetLostFocus();
            }
        }
    }
}
