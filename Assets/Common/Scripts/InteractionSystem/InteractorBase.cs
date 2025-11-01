#nullable enable

using System;
using UnityEngine;

namespace KarenKrill.Interactions
{
    using Abstractions;

    public abstract class InteractorBase : MonoBehaviour, IInteractor
    {
        public event Action<IInteractable?>? Interaction;
        public event Action<IInteractable, bool>? InteractionAvailabilityChanged;

        public void Interact(IInteractable? interactable)
        {
            OnInteraction(interactable);
            Interaction?.Invoke(interactable);
        }
        public void SetInteractionAvailability(IInteractable interactable, bool available = true)
        {
            OnInteractionAvailabilityChanged(interactable, available);
            InteractionAvailabilityChanged?.Invoke(interactable, available);
        }

        protected abstract void OnInteraction(IInteractable? interactable);
        protected abstract void OnInteractionAvailabilityChanged(IInteractable interactable, bool available);
    }
}
