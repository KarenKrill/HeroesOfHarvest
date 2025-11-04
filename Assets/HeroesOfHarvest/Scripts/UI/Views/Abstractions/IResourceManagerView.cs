using UnityEngine;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Views.Abstractions
{
	public interface IResourceManagerView : IView
	{
        ResourceManagerItem[] Items { set; }
	}
    public readonly struct ResourceManagerItem
    {
        public readonly Sprite icon;
        public readonly string amountText;

        public ResourceManagerItem(Sprite Icon, string AmountText)
        {
            icon = Icon;
            amountText = AmountText;
        }
    }
}