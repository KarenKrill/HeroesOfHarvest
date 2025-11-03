using UnityEngine;

using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    public class BuildInteractable : OutlineInteractableBase, IInteractable
    {
        [Inject]
        public void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        protected override bool OnInteraction(IInteractor interactor)
        {
            _logger.Log($"{name} build interaction");
            return true;
        }

        private ILogger _logger;
    }
}
