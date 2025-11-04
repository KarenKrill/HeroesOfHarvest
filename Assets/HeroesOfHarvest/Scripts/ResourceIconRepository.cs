using System.Collections.Generic;
using UnityEngine;

using AYellowpaper.SerializedCollections;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest
{
    [CreateAssetMenu(fileName = nameof(ResourceIconRepository), menuName = "Scriptable Objects/" + nameof(ResourceIconRepository))]
    public class ResourceIconRepository : ScriptableObject, IResourceIconRepository
    {
        public IReadOnlyDictionary<ResourceType, Sprite> Icons => _icons;

        [SerializeField, SerializedDictionary("Type", "Icon")]
        private SerializedDictionary<ResourceType, Sprite> _icons = new()
        {
            { ResourceType.Water, null },
            { ResourceType.Rye, null },
            { ResourceType.Wood, null },
            { ResourceType.Stone, null },
            { ResourceType.Iron, null },
            { ResourceType.Gold, null },
            { ResourceType.Crystal, null }
        };
    }
}
