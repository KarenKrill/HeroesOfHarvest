using System;
using UnityEngine;
using UnityEngine.UI;

using KarenKrill.UniCore.UI.Views;

namespace HeroesOfHarvest.UI.Views
{
    using Abstractions;

    public class PauseMenuView : ViewBehaviour, IPauseMenuView
    {
#nullable enable
        public event Action? ResumeRequested;
        public event Action? RestartRequested;
        public event Action? SettingsOpenRequested;
        public event Action? MainMenuExitRequested;
        public event Action? ExitRequested;
#nullable restore

        [SerializeField]
        private Button _resumeButton;
        [SerializeField]
        private Button _restartButton;
        [SerializeField]
        private Button _settingsButton;
        [SerializeField]
        private Button _mainMenuExitButton;
        [SerializeField]
        private Button _exitButton;

        private void OnEnable()
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            _mainMenuExitButton.onClick.AddListener(OnMainMenuExitButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        private void OnDisable()
        {
            _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            _mainMenuExitButton.onClick.RemoveListener(OnMainMenuExitButtonClicked);
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        private void OnResumeButtonClicked() => ResumeRequested?.Invoke();
        private void OnRestartButtonClicked() => RestartRequested?.Invoke();
        private void OnSettingsButtonClicked() => SettingsOpenRequested?.Invoke();
        private void OnMainMenuExitButtonClicked() => MainMenuExitRequested?.Invoke();
        private void OnExitButtonClicked() => ExitRequested?.Invoke();
    }
}