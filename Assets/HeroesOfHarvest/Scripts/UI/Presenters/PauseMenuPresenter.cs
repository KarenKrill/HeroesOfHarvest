using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Presenters
{
    using Abstractions;
    using HeroesOfHarvest.Abstractions;
    using Views.Abstractions;

    public class PauseMenuPresenter : PresenterBase<IPauseMenuView>, IPauseMenuPresenter, IPresenter<IPauseMenuView>
    {
#nullable enable
        public event Action? Resume;
        public event Action? Restart;
        public event Action? MainMenu;
        public event Action? Exit;
#nullable restore

        public PauseMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            GameSettings gameSettings) : base(viewFactory, navigator)
        {
            _settingsPresenter = new SettingsMenuPresenter(viewFactory, navigator, gameSettings);
        }

        protected override void Subscribe()
        {
            View.ResumeRequested += OnResume;
            View.RestartRequested += OnRestart;
            View.SettingsOpenRequested += OnSettings;
            View.MainMenuExitRequested += OnMainMenuExit;
            View.ExitRequested += OnExit;
        }
        protected override void Unsubscribe()
        {
            View.ResumeRequested -= OnResume;
            View.RestartRequested -= OnRestart;
            View.SettingsOpenRequested -= OnSettings;
            View.MainMenuExitRequested -= OnMainMenuExit;
            View.ExitRequested -= OnExit;
            Navigator.Clear();
        }

        private readonly ISettingsMenuPresenter _settingsPresenter;

        private void OnResume() => Resume?.Invoke();
        private void OnRestart() => Restart?.Invoke();
        private void OnSettings()
        {
            _settingsPresenter.Close += OnSettingsClose;
            Navigator.Push(_settingsPresenter);
        }
        private void OnSettingsClose()
        {
            _settingsPresenter.Close -= OnSettingsClose;
            Navigator.Pop();
        }
        private void OnMainMenuExit() => MainMenu?.Invoke();
        private void OnExit() => Exit?.Invoke();
    }
}