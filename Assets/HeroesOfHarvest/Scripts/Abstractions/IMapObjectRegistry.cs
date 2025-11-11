#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HeroesOfHarvest.Abstractions
{
    public interface IMapObjectRegistry
    {
        IReadOnlyDictionary<IMapObjectId, IMapObject> RegistredObjects { get; }

        void Register(IMapObjectId id, IMapObject mapObject);
        void Unregister(IMapObjectId id);
        bool TryGetMapObject(IMapObjectId mapObjectId, [NotNullWhen(true)] out IMapObject? mapObject);
    }
}
