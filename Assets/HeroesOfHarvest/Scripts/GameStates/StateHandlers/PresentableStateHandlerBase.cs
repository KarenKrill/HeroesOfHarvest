#nullable enable

using System;
using System.Linq;

using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    public abstract class PresentableStateHandlerBase<T> : IStateHandler<T> where T : Enum
    {
        public abstract T State { get; }

        public PresentableStateHandlerBase(params IPresenter[] presenters)
        {
            _presenters = presenters.ToArray();
        }
        public virtual void Enter(T prevState, object? context = null)
        {
            foreach (var presenter in _presenters)
            {
                presenter.Enable();
            }
        }
        public virtual void Exit(T nextState)
        {
            foreach (var presenter in _presenters)
            {
                presenter.Disable();
            }
        }

        private readonly IPresenter[] _presenters;
    }
}
