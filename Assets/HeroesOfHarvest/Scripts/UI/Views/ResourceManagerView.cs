using System.Collections.Generic;
using UnityEngine;

using KarenKrill.UniCore.Instantiattion;
using KarenKrill.UniCore.UI.Views;

namespace HeroesOfHarvest.UI.Views
{
    using Abstractions;

    public class ResourceManagerView : ViewBehaviour, IResourceManagerView
    {
        public ResourceManagerItem[] Items { set => UpdateItems(value); }

        [SerializeField]
        private ResourceManagerItemView _itemPrefab;
        [SerializeField]
        private Transform _itemsParentTransform;
        [SerializeField]
        private int _defaultItemsCapacity = 10;

        private ComponentPool<ResourceManagerItemView> _itemsPool;
        private readonly List<ResourceManagerItemView> _currentItems = new();

        private void Awake()
        {
            _itemsPool = new(_itemPrefab, _itemsParentTransform, _defaultItemsCapacity);
        }
        private void OnDestroy()
        {
            _itemsPool.Dispose();
        }
        private void UpdateItems(ResourceManagerItem[] itemsData)
        {
            if (_currentItems.Count != itemsData.Length)
            {
                while (_currentItems.Count > itemsData.Length)
                {
                    var lastItem = _currentItems[^1];
                    _currentItems.Remove(lastItem);
                    _itemsPool.Release(lastItem);
                }
                while (_currentItems.Count < itemsData.Length)
                {
                    var item = _itemsPool.Get();
                    _currentItems.Add(item);
                }
            }
            for (int i = 0; i < itemsData?.Length; i++)
            {
                var item = _currentItems[i];
                item.Icon = itemsData[i].icon;
                item.AmountText = itemsData[i].amountText;
            }
        }
    }
}