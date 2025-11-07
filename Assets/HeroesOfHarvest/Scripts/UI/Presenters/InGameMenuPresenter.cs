using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

namespace HeroesOfHarvest.UI.Presenters
{
    using Abstractions;
    using Views.Abstractions;

    public class InGameMenuPresenter : PresenterBase<IInGameMenuView>, IInGameMenuPresenter, IPresenter<IInGameMenuView>
    {
#nullable enable
        public event Action? Pause;
#nullable restore

        public InGameMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator) : base(viewFactory, navigator)
        {
        }

        protected override void Subscribe()
        {
            View.PauseRequested += OnPause;
        }
        protected override void Unsubscribe()
        {
            View.PauseRequested -= OnPause;
        }

        private void OnPause() => Pause?.Invoke();
    }
}