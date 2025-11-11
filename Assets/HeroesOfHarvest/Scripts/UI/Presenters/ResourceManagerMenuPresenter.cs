using System;
using System.Collections.Generic;
using UnityEngine;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

using HeroesOfHarvest.Abstractions;
using HeroesOfHarvest.UI.Views.Abstractions;
using System.Linq;

namespace HeroesOfHarvest.UI.Presenters
{
    public class ResourceManagerMenuPresenter : PresenterBase<IResourceManagerView>, IPresenter<IResourceManagerView>
    {
        public ResourceManagerMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            IResourceManager resourceManager,
            IResourceIconRepository resourceIconRepository) : base(viewFactory, navigator)
        {
            _resourceManager = resourceManager;
            _resourceIconRepository = resourceIconRepository;
        }

        protected override void Subscribe()
        {
            UpdateResourcesIfNeeded();
            _resourceManager.ResourceChanged += OnResourceChanged;
        }
        protected override void Unsubscribe()
        {
            _resourceManager.ResourceChanged -= OnResourceChanged;
        }
        
        private readonly IResourceManager _resourceManager;
        private readonly IResourceIconRepository _resourceIconRepository;
        private ResourceManagerItem[] _resourcesViewInfo = Array.Empty<ResourceManagerItem>();

        private void OnResourceChanged(ResourceType type, int amount)
        {
            UpdateResourcesIfNeeded();
        }
        private void UpdateResourcesIfNeeded()
        {
            var resources = _resourceManager.GetResources();
            // TODO: Partial change functionality should be implemented here by refactoring ResourceManagerMenuItem and ResourceManagerMenuView
            RefreshResources(resources);
        }
        private void RefreshResources(IReadOnlyDictionary<ResourceType, int> resources)
        {
            _resourcesViewInfo = new ResourceManagerItem[resources.Count];
            int resViewInfoIndex = 0;
            // can be optimized
            var sortedResources = resources.OrderBy(resource => (int)resource.Key);
            foreach (var resource in sortedResources)
            {
                var amountText = resource.Value.ToString();
                if (!_resourceIconRepository.Icons.TryGetValue(resource.Key, out var icon))
                {
                    icon = null;
                }
                _resourcesViewInfo[resViewInfoIndex++] = new(icon, amountText);
            }
            View.Items = _resourcesViewInfo;
        }
    }
}