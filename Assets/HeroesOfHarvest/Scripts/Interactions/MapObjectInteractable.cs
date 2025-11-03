using KarenKrill.UniCore.Interactions.Abstractions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace HeroesOfHarvest.Interactions
{
    public class MapObjectInteractable : OutlineInteractableBase, IInteractable
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
