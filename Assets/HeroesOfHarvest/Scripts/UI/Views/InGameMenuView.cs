using System;
using UnityEngine;
using UnityEngine.UI;

using KarenKrill.UniCore.UI.Views;

namespace HeroesOfHarvest.UI.Views
{
    using Abstractions;

    public class InGameMenuView : ViewBehaviour, IInGameMenuView
    {
#nullable enable
        public event Action? PauseRequested;
#nullable restore

        [SerializeField]
        private Button _pauseButton;

        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        }

        private void OnPauseButtonClicked() => PauseRequested?.Invoke();
    }
}