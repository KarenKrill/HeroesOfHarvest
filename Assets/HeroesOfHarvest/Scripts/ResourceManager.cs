using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest
{
    public class ResourceManager : IResourceManager
    {
        public event Action<ResourceType, int> ResourceChanged;
        public IReadOnlyDictionary<ResourceType, int> Resources => _resources;
        public bool FreezeResourceChanged { get; set; } = false;

        public ResourceManager()
        {
            var resTypes = Enum.GetValues(typeof(ResourceType));
            foreach (ResourceType resType in resTypes)
            {
                _resources.Add(resType, 0);
            }
        }
        public void AddResource(ResourceType resourceType, int amount)
        {
            _resources[resourceType] += amount;
            if (!FreezeResourceChanged)
            {
                ResourceChanged?.Invoke(resourceType, _resources[resourceType]);
            }
        }
        public void RemoveResource(ResourceType resourceType, int amount)
        {
            _resources[resourceType] -= amount;
            if (!FreezeResourceChanged)
            {
                ResourceChanged?.Invoke(resourceType, _resources[resourceType]);
            }
        }

        private readonly Dictionary<ResourceType, int> _resources = new();
    }
}
