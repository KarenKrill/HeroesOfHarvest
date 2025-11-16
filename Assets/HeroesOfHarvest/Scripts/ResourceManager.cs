using System;
using System.Collections.Generic;

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
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Shouldn't be negative");
            }
            if (!_resources.ContainsKey(resourceType))
            {
                _resources[resourceType] = amount;
            }
            else
            {
                _resources[resourceType] += amount;
            }
            if (!FreezeResourceChanged)
            {
                ResourceChanged?.Invoke(resourceType, amount);
            }
        }
        public void RemoveResource(ResourceType resourceType, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Shouldn't be negative");
            }
            _resources[resourceType] -= amount;
            if (!FreezeResourceChanged)
            {
                ResourceChanged?.Invoke(resourceType, amount);
            }
        }

        private readonly Dictionary<ResourceType, int> _resources = new();
    }
}
