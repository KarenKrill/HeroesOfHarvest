using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions;
using KarenKrill.UniCore.Interactions.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    public class PlayerInteractor : InteractorBase, IInteractor
    {
        [Inject]
        public void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        protected override void OnInteraction(IInteractable interactable)
        {
            if (interactable is InteractableBase interactableBase)
            {
                _logger.Log($"{name} interacts with {interactableBase.name}");
            }
        }
        protected override void OnInteractionAvailabilityChanged(IInteractable interactable, bool available)
        {
            _logger.Log($"{name} {(available ? "can" : "can't")} interact with {(interactable as InteractableBase).name}");
        }

        private ILogger _logger;
    }
}
