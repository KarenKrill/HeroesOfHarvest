#nullable enable

using System;

namespace KarenKrill.Interactions.Abstractions
{
    public interface IInteractable
    {
        event Action<IInteractor>? Interaction;
        event Action<IInteractor, bool>? InteractionAvailabilityChanged;

        /// <summary></summary>
        /// <param name="interactor"></param>
        /// <returns><see langword="true"/> if interaction allowed, <see langword="false"/> otherwise</returns>
        bool Interact(IInteractor interactor);
        void SetInteractionAvailability(IInteractor interactor, bool available = true);
    }
}
