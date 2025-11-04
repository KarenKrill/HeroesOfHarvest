using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions;
using KarenKrill.UniCore.Interactions.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    public class PlayerInteractor : InteractorBase, IInteractor
    {
        [Inject]
        public void Initialize(ILogger logger, IUnitMover unitMover)
        {
            _logger = logger;
            _unitMover = unitMover;
        }

        protected override void OnInteraction(IInteractable interactable)
        {
            if (interactable is InteractableBase interactableBase)
            {
                _logger.Log($"{name} interacts with {interactableBase.name}");
                if (interactable is MapObjectInteractable mapObjectInteractable)
                {
                    _unitMover.PathCancelled += OnUnitPathCancelled;
                    _unitMover.PathCompleted += OnUnitPathCompleted;
                    _unitMover.GoTo(mapObjectInteractable.EntryTransform.position);
                }
            }
        }
        protected override void OnInteractionAvailabilityChanged(IInteractable interactable, bool available)
        {
            _logger.Log($"{name} {(available ? "can" : "can't")} interact with {(interactable as InteractableBase).name}");
        }

        private ILogger _logger;
        private IUnitMover _unitMover;

        private void OnUnitPathCancelled()
        {
            _unitMover.PathCancelled -= OnUnitPathCancelled;
            _unitMover.PathCompleted -= OnUnitPathCompleted;
            Debug.LogWarning("Path cancelled");
        }
        private void OnUnitPathCompleted()
        {
            _unitMover.PathCancelled -= OnUnitPathCancelled;
            _unitMover.PathCompleted -= OnUnitPathCompleted;
            Debug.LogWarning("Path completed");
        }
    }
}
