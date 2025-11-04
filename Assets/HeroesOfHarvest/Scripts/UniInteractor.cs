using System;
using System.Collections;
using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Abstractions;
using HeroesOfHarvest.Interactions;

namespace HeroesOfHarvest
{
    public class UniInteractor : MonoBehaviour, IUnit, IInteractor
    {
        public event Action<IInteractable> Interaction;
        public event Action<IInteractable, bool> InteractionAvailabilityChanged;

        public bool IsBusy => _isBusy;

        [Inject]
        public void Initialize(IPlayerSession playerSession)
        {
            _playerSession = playerSession;
        }
        public void Interact(IInteractable interactable)
        {
            if (!_isBusy && interactable is MapObjectInteractable mapObjectInteractable)
            {
                _isBusy = true;
                if (interactable is ResourceFactoryInteractable resourceFactoryInteractable)
                {
                    StartCoroutine(CollectResourcesCoroutine(resourceFactoryInteractable));
                }
                Interaction?.Invoke(interactable);
            }
        }
        public void SetInteractionAvailability(IInteractable interactable, bool available = true)
        {
            InteractionAvailabilityChanged?.Invoke(interactable, available);
        }

        private IPlayerSession _playerSession;
        private bool _isBusy = false;

        private IEnumerator CollectResourcesCoroutine(ResourceFactoryInteractable resourceFactoryInteractable)
        {
            Debug.LogWarning("Unit animation started");
            var resourceToConsumeCount = resourceFactoryInteractable.ResourceAmount;
            for (int i = 0; i < resourceToConsumeCount; i++)
            {
                _playerSession.ResourceManager.AddResource(resourceFactoryInteractable.ProducedResource, 1);
                resourceFactoryInteractable.ResourceAmount--;
                yield return new WaitForSeconds(.25f);
            }
            Debug.LogWarning("Unit animation finished");
            _isBusy = false;
        }
    }
}
