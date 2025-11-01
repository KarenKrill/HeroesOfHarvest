using UnityEngine;
using Zenject;

namespace KarenKrill.Interactions
{
    using Abstractions;

    public abstract class InteractionTargetBase : MonoBehaviour, IInteractionTarget
    {
        public abstract IInteractable Interactable { get; }
        
        [Inject]
        public void Initialize(IInteractionTargetRegistry interactionTargetRegistry)
        {
            _interactionTargetRegistry = interactionTargetRegistry;
        }

        protected virtual void OnEnable()
        {
            _interactionTargetRegistry.Register(this);
        }
        protected virtual void OnDisable()
        {
            _interactionTargetRegistry.Unregister(this);
        }

        private IInteractionTargetRegistry _interactionTargetRegistry;
    }
}
