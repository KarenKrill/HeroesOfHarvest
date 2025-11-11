using System;

namespace HeroesOfHarvest.Abstractions
{
    public interface IMapObject
    {
        event Action StateChanged;
        IMapObjectId Id { get; }
    }
}
