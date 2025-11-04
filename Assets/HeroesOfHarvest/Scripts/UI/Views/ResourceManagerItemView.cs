using UnityEngine;
using UnityEngine.UI;
using TMPro;

using KarenKrill.UniCore.UI.Views;

namespace HeroesOfHarvest.UI.Views
{
    using Abstractions;

    public class ResourceManagerItemView : ViewBehaviour, IResourceManagerItemView
    {
        public Sprite Icon { set => _resourceIcon.sprite = value; }
        public string AmountText { set => _amountText.text = value; }

        [SerializeField]
        private Image _resourceIcon;
        [SerializeField]
        private TextMeshProUGUI _amountText;
    }
}