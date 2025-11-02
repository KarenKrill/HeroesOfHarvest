#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

namespace HeroesOfHarvest.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public interface ISettingsMenuPresenter : IPresenter<ISettingsMenuView>
    {
        public event Action? Close;
    }
}
