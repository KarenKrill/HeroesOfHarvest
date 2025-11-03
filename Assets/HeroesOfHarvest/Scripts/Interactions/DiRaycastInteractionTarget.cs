using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;
using KarenKrill.UniCore.Interactions;

namespace HeroesOfHarvest.Interactions
{
    public class DiRaycastInteractionTarget : RaycastInteractionTarget, IInteractionTarget
    {
        [Inject]
        public override void Initialize(IInteractionTargetRegistry interactionTargetRegistry)
        {
            base.Initialize(interactionTargetRegistry);
        }
    }
}
