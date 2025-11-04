using System.Collections.Generic;
using UnityEngine;

namespace HeroesOfHarvest.Abstractions
{
    public interface IResourceIconRepository
    {
        IReadOnlyDictionary<ResourceType, Sprite> Icons { get; }
    }
}
