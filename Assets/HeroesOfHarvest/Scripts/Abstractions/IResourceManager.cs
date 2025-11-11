using System;
using System.Collections.Generic;

namespace HeroesOfHarvest.Abstractions
{
    public interface IResourceManager
    {
        event Action<ResourceType, int> ResourceChanged;

        IReadOnlyDictionary<ResourceType, int> GetResources();
        void AddResource(ResourceType resourceType, int amount);
        void RemoveResource(ResourceType resourceType, int amount);
    }
}
