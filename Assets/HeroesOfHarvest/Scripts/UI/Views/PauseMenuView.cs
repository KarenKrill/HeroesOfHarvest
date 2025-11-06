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

#if UNITY_WEBGL
        private void Awake()
        {
            _exitButton.gameObject.SetActive(false);
            Destroy(_exitButton);
        }
#endif
        private void OnEnable()
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            _mainMenuExitButton.onClick.AddListener(OnMainMenuExitButtonClicked);
#if !UNITY_WEBGL
            _exitButton.onClick.AddListener(OnExitButtonClicked);
#endif
        }
        private void OnDisable()
        {
            _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            _mainMenuExitButton.onClick.RemoveListener(OnMainMenuExitButtonClicked);
#if !UNITY_WEBGL
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
#endif
        }

        private void OnResumeButtonClicked() => ResumeRequested?.Invoke();
        private void OnRestartButtonClicked() => RestartRequested?.Invoke();
        private void OnSettingsButtonClicked() => SettingsOpenRequested?.Invoke();
        private void OnMainMenuExitButtonClicked() => MainMenuExitRequested?.Invoke();
#if !UNITY_WEBGL
        private void OnExitButtonClicked() => ExitRequested?.Invoke();
#endif
    }
}