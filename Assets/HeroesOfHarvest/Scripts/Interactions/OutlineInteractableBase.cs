using UnityEngine;

using KarenKrill.UniCore.Interactions;
using KarenKrill.UniCore.Interactions.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    [RequireComponent(typeof(Outline))]
    public abstract class OutlineInteractableBase : InteractableBase, IInteractable
    {
        protected override void OnInteractionAvailabilityChanged(bool available)
        {
            _outline.enabled = available;
        }

        private Outline _outline;

        protected virtual void Awake()
        {
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
        }
    }
}
