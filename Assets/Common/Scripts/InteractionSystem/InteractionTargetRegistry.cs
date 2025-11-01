using System;
using System.Collections;
using System.Collections.Generic;

namespace KarenKrill.Interactions
{
    using Abstractions;

    public class InteractionTargetRegistry : IInteractionTargetRegistry
    {
        public event Action<IInteractionTarget> Registred;
        public event Action<IInteractionTarget> Unregistred;

        public void Register(IInteractionTarget target)
        {
            _interactionTargets.Add(target);
            Registred?.Invoke(target);
        }
        public void Unregister(IInteractionTarget target)
        {
            _interactionTargets.Remove(target);
            Unregistred?.Invoke(target);
        }
        public IEnumerator GetEnumerator() => _interactionTargets.GetEnumerator();
        public bool Contains(IInteractionTarget target) => _interactionTargets.Contains(target);

        private readonly HashSet<IInteractionTarget> _interactionTargets = new();
    }
}
