using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

using AYellowpaper.SerializedCollections;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest
{
    public class MapObjectRegistry : IMapObjectRegistry, IStringSerializable
    {
        public IReadOnlyDictionary<IMapObjectId, IMapObject> RegistredObjects => _registredObjects;

        public void Register(IMapObjectId id, IMapObject mapObject)
        {
            if (id is IStringSerializable serializableId && mapObject is IStringSerializable serializableMapObject)
            {
                var serializedId = serializableId.ToSerializedString();
                if (_serializedRegistredObjects.TryGetValue(serializedId, out var serializedMapObject))
                {
                    serializableMapObject.FromSerializedString(serializedMapObject);
                }
            }
            _registredObjects.Add(id, mapObject);
        }
        public void Unregister(IMapObjectId id)
        {
            _registredObjects.Remove(id);
        }
        public bool TryGetMapObject(IMapObjectId mapObjectId, [NotNullWhen(true)] out IMapObject mapObject)
        {
            return _registredObjects.TryGetValue(mapObjectId, out mapObject);
        }

        public string ToSerializedString()
        {
            var serializedRegistredObjects = new SerializedDictionary<string, string>();
            foreach (var registredObject in _registredObjects)
            {
                if (registredObject.Key is IStringSerializable serializableMapObjectId)
                {
                    if (registredObject.Value is IStringSerializable serializableMapObject)
                    {
                        var key = serializableMapObjectId.ToSerializedString();
                        var value = serializableMapObject.ToSerializedString();
                        serializedRegistredObjects[key] = value;
                    }
                }
            }
            return JsonUtility.ToJson(serializedRegistredObjects);
        }
        public void FromSerializedString(string serializedString)
        {
            var serializedRegistredObjects = JsonUtility.FromJson<SerializedDictionary<string, string>>(serializedString);
            _serializedRegistredObjects.Clear();
            foreach (var (key, value) in serializedRegistredObjects)
            {
                _serializedRegistredObjects.Add(key, value);
            }
        }

        private readonly Dictionary<IMapObjectId, IMapObject> _registredObjects = new();
        private readonly Dictionary<string, string> _serializedRegistredObjects = new();
    }
}
