using System;
using System.Collections;
using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Abstractions;
using HeroesOfHarvest.Interactions;

namespace HeroesOfHarvest
{
    public class UnitInteractor : MonoBehaviour, IUnit, IInteractor
    {
        public event Action<IInteractable> Interaction;
        public event Action<IInteractable, bool> InteractionAvailabilityChanged;

        public bool IsBusy => _isBusy;
        public Vector3 WorldPosition => transform.position;

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

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float _miningOneResourceUnitPeriodTime = .5f;

        private IPlayerSession _playerSession;
        private bool _isBusy = false;

        private IEnumerator CollectResourcesCoroutine(ResourceFactoryInteractable resourceFactory)
        {
            resourceFactory.ProducingEnabled = false;
            try
            {
                _animator.SetTrigger(resourceFactory.BuildConfig.MiningAnimationTrigger);
                try
                {
                    var resourceToConsumeCount = resourceFactory.ResourceAmount;
                    for (int i = 0; i < resourceToConsumeCount; i++)
                    {
                        _playerSession.ResourceManager.AddResource(resourceFactory.BuildConfig.ProducedResource, 1);
                        resourceFactory.ResourceAmount--;
                        yield return new WaitForSeconds(_miningOneResourceUnitPeriodTime);
                    }
                }
                finally
                {
                    _animator.SetTrigger(resourceFactory.BuildConfig.MiningStopAnimationTrigger);
                }
            }
            finally
            {
                _isBusy = false;
                resourceFactory.ProducingEnabled = true;
            }
        }
    }
}
