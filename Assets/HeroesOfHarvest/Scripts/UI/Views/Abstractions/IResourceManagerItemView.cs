using UnityEngine;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Views.Abstractions
{
    public interface IResourceManagerItemView : IView
    {
        public Sprite Icon { set; }
        public string AmountText { set; }
    }
}