#nullable enable

using System;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Views.Abstractions
{
    public interface IPauseMenuView : IView
    {
        public event Action? ResumeRequested;
        public event Action? SettingsOpenRequested;
        public event Action? RestartRequested;
        public event Action? MainMenuExitRequested;
        public event Action? ExitRequested;
    }
}