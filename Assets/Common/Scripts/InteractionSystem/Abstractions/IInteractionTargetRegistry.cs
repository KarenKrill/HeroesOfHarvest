#nullable enable

using System;
using System.Collections;

namespace KarenKrill.Interactions.Abstractions
{
    public interface IInteractionTargetRegistry
    {
        event Action<IInteractionTarget>? Registred;
        event Action<IInteractionTarget>? Unregistred;

        void Register(IInteractionTarget interactionTarget);
        void Unregister(IInteractionTarget interactionTarget);
        IEnumerator GetEnumerator();
        bool Contains(IInteractionTarget interactionTarget);
    }
}
