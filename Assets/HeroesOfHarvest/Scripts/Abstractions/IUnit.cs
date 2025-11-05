using UnityEngine;

namespace HeroesOfHarvest.Abstractions
{
    public interface IUnit
    {
        bool IsBusy { get; }
        Vector3 WorldPosition { get; }
    }
}
