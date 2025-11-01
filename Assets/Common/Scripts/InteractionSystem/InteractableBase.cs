#nullable enable

using System;
using UnityEngine;

namespace KarenKrill.Interactions
{
    using Abstractions;

    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        public event Action<IInteractor>? Interaction;
        public event Action<IInteractor, bool>? InteractionAvailabilityChanged;

        public bool Interact(IInteractor interactor)
        {
            var isInteractionAllowed = OnInteraction(interactor);
            if (isInteractionAllowed)
            {
                Interaction?.Invoke(interactor);
            }
            return isInteractionAllowed;
        }
        public void SetInteractionAvailability(IInteractor interactor, bool available = true)
        {
            OnInteractionAvailabilityChanged(available);
            InteractionAvailabilityChanged?.Invoke(interactor, available);
        }

        protected abstract bool OnInteraction(IInteractor interactor);
        protected abstract void OnInteractionAvailabilityChanged(bool available);
    }
}
