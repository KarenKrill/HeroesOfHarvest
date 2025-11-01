using System.Collections.Generic;
using UnityEngine;

namespace KarenKrill.UI.Views
{
    using Abstractions;

    public class ViewFactory : IViewFactory
    {
        public ViewFactory(GameObject parentGameObject, List<GameObject> viewPrefabs)
        {
            _parentGameObject = parentGameObject;
            _viewPrefabs = viewPrefabs;
        }
        public ViewType Create<ViewType>() where ViewType : IView
        {
            foreach (var viewPrefab in _viewPrefabs)
            {
                if (viewPrefab.TryGetComponent<ViewType>(out _))
                {
                    var viewGameObject = Object.Instantiate(viewPrefab, _parentGameObject.transform);
                    viewGameObject.SetActive(false);
                    viewGameObject.name = viewPrefab.name;
                    return viewGameObject.GetComponent<ViewType>();
                }
            }
            throw new System.InvalidOperationException($"There is no prefab for \"{typeof(ViewType).Name}\" view");
        }

        private readonly GameObject _parentGameObject;
        private readonly List<GameObject> _viewPrefabs = new();
    }
}