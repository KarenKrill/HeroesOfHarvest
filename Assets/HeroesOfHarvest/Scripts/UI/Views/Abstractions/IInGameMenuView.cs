#nullable enable

using System;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Views.Abstractions
{
    public interface IInGameMenuView : IView
    {
        public event Action? PauseRequested;
    }
}