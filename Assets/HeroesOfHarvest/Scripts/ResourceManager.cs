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

        public ResourceManager()
        {
            var resTypes = Enum.GetValues(typeof(ResourceType));
            foreach (ResourceType resType in resTypes)
            {
                _resources.Add(resType, 0);
            }
        }
        public IReadOnlyDictionary<ResourceType, int> GetResources() => _resources;
        public void AddResource(ResourceType resourceType, int amount)
        {
            _resources[resourceType] += amount;
            ResourceChanged?.Invoke(resourceType, _resources[resourceType]);
            PrintResources();
        }
        public void RemoveResource(ResourceType resourceType, int amount)
        {
            _resources[resourceType] -= amount;
            ResourceChanged?.Invoke(resourceType, _resources[resourceType]);
        }

        private readonly Dictionary<ResourceType, int> _resources = new();

        private void PrintResources()
        {
            Debug.Log($"Resources: {string.Join(", ", _resources.Select(pair => $"[{pair.Key}:{pair.Value}]"))}");
        }
    }
}
