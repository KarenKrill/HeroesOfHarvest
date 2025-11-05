#nullable enable

using System;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Views.Abstractions
{
    public interface ISettingsMenuView : IView
    {
        #region Graphics

        #endregion

        #region Music
        float MusicVolume { get; set; }
        #endregion

        #region Diagnostic
        bool ShowFps { get; set; }
        #endregion

        event Action? ApplyRequested;
        event Action? CancelRequested;
    }
}