#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

namespace HeroesOfHarvest.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public interface IPauseMenuPresenter : IPresenter<IPauseMenuView>
    {
        public event Action? Resume;
        public event Action? Restart;
        public event Action? MainMenu;
        public event Action? Exit;
    }
}
