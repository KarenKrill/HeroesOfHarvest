using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions;
using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Movement;
using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    public class PlayerInteractor : InteractorBase, IInteractor
    {
        [Inject]
        public void Initialize(ILogger logger, IUnitMover unitMover, IPlayerSession playerSession)
        {
            _logger = logger;
            _unitMover = unitMover;
            _playerSession = playerSession;
        }

        protected override void OnInteraction(IInteractable interactable)
        {
            if (interactable is InteractableBase interactableBase)
            {
                _logger.Log($"{name} interacts with {interactableBase.name}");
                if (interactable is MapObjectInteractable mapObjectInteractable)
                {
                    if (_unitMover.TrySend(_playerSession.ActiveUnit, mapObjectInteractable.EntryTransform.position))
                    {
                        _lastInteractable = mapObjectInteractable;
                        _unitMover.UnitPathCancelled += OnUnitPathCancelled;
                        _unitMover.UnitPathCompleted += OnUnitPathCompleted;
                    }
                }
            }
        }
        protected override void OnInteractionAvailabilityChanged(IInteractable interactable, bool available)
        {
            _logger.Log($"{name} {(available ? "can" : "can't")} interact with {(interactable as InteractableBase).name}");
        }

        private ILogger _logger;
        private IUnitMover _unitMover;
        private IPlayerSession _playerSession;
        private MapObjectInteractable _lastInteractable;

        private void OnUnitPathCancelled(IUnit unit)
        {
            _unitMover.UnitPathCancelled -= OnUnitPathCancelled;
            _unitMover.UnitPathCompleted -= OnUnitPathCompleted;
        }
        private void OnUnitPathCompleted(IUnit unit)
        {
            _unitMover.UnitPathCancelled -= OnUnitPathCancelled;
            _unitMover.UnitPathCompleted -= OnUnitPathCompleted;
            if (unit is IInteractor unitInteractor)
            {
                if (_lastInteractable.Interact(unitInteractor))
                {
                    unitInteractor.Interact(_lastInteractable);
                }
            }
        }
    }
}
