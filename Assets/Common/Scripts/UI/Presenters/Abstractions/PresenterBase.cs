namespace KarenKrill.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public abstract class PresenterBase<T> : IPresenter<T> where T : IView
    {
        public PresenterBase(IViewFactory viewFactory, IPresenterNavigator navigator)
        {
            _viewFactory = viewFactory;
            Navigator = navigator;
        }

        public virtual void Enable()
        {
            View ??= _viewFactory.Create<T>();
            Subscribe();
            View.Show();
            _isEnabled = true;
        }
        public virtual void Disable()
        {
            if (_isEnabled)
            {
                Unsubscribe();
                View.Close();
                _isEnabled = false;
            }
        }

        /// <summary>
        /// <see cref="View"/> Use only after <see cref="Subscribe"/> and before <see cref="Unsubscribe"/>, otherwise can be null
        /// </summary>
        protected T View { get; private set; }
        protected IPresenterNavigator Navigator { get; private set; }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();

        private readonly IViewFactory _viewFactory;
        private bool _isEnabled = false;
    }
}
