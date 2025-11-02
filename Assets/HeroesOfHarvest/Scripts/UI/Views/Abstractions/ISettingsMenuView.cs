#nullable enable

using System;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Views.Abstractions
{
    public interface ISettingsMenuView : IView
    {
        #region Graphics

        #endregion

        #region Diagnostic
        bool ShowFps { get; set; }
        #endregion

        event Action? ApplyRequested;
        event Action? CancelRequested;
    }
}