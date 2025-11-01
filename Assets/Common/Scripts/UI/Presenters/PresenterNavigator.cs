using System.Collections.Generic;

namespace KarenKrill.UI.Presenters
{
    using Abstractions;

    public class PresenterNavigator : IPresenterNavigator
    {
        public void Push(IPresenter presenter)
        {
            _presenters.Push(presenter);
            presenter.Enable();
        }
        public void Pop()
        {
            if (_presenters.Count > 0)
            {
                var presenter = _presenters.Pop();
                presenter.Disable();
            }
        }
        public void Clear()
        {
            foreach (var presenter in _presenters)
            {
                presenter.Disable();
            }
            _presenters.Clear();
        }

        private readonly Stack<IPresenter> _presenters = new();
    }
}
