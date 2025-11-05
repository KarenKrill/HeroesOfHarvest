using System;
using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;
using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    [Serializable]
    public class MapObjectInteractable : OutlineInteractableBase, IMapObject, IInteractable, IStringSerializable
    {
        [field: SerializeField]
        public Transform EntryTransform { get; private set; }
        public IMapObjectId Id => _staticMapObjectId;

        [Inject]
        public void Initialize(ILogger logger, IMapObjectRegistry mapObjectRegistry)
        {
            _logger = logger;
            _mapObjectRegistry = mapObjectRegistry;
        }

        protected override void Awake()
        {
            _staticMapObjectId = new(transform.position);
            if (!_mapObjectRegistry.TryGetMapObject(_staticMapObjectId, out _))
            {
                _mapObjectRegistry.Register(_staticMapObjectId, this);
            }
            base.Awake();
        }
        protected void OnDestroy()
        {
            if (_mapObjectRegistry.TryGetMapObject(_staticMapObjectId, out _))
            {
                _mapObjectRegistry.Unregister(_staticMapObjectId);
            }
            base.Awake();
        }
        protected override bool OnInteraction(IInteractor interactor)
        {
            _logger.Log($"{name} build interaction");
            return true;
        }

        public virtual string ToSerializedString() => _staticMapObjectId.ToSerializedString();
        public virtual void FromSerializedString(string serializedString)
        {
            _staticMapObjectId ??= new(Vector3.zero);
            _staticMapObjectId.FromSerializedString(serializedString);
        }

        private ILogger _logger;
        [SerializeField, HideInInspector]
        private StaticMapObjectId _staticMapObjectId;
        private IMapObjectRegistry _mapObjectRegistry;
    }
}
