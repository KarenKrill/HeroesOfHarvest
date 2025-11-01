#nullable enable

using System;

namespace KarenKrill.Interactions.Abstractions
{
    public interface IInteractor
    {
        event Action<IInteractable?>? Interaction;
        event Action<IInteractable, bool>? InteractionAvailabilityChanged;

        void Interact(IInteractable? interactable);
        void SetInteractionAvailability(IInteractable interactable, bool available = true);
    }
}
