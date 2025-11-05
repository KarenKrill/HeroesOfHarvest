using UnityEngine;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    [CreateAssetMenu(fileName = nameof(FactoryBuildConfig), menuName = "Scriptable Objects/" + nameof(FactoryBuildConfig))]
    public class FactoryBuildConfig : ScriptableObject
    {
        [field: SerializeField]
        public ResourceType ProducedResource { get; private set; }
        [field: SerializeField]
        public int ResourceAmountPerSecond { get; private set; } = 1;
        [field: SerializeField]
        public int MaxResourceAmount { get; private set; } = 20;
        [field: SerializeField]
        public string MiningAnimationTrigger { get; private set; } = "StartDigging";
        [field: SerializeField]
        public string MiningStopAnimationTrigger { get; private set; } = "StopWork";
        [field: SerializeField]
        public AudioClip MiningSoundEffect { get; private set; }
    }
}
