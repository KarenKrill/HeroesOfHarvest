#nullable enable

using System;
using System.Collections.Generic;

namespace KarenKrill.StateSystem
{
    using Abstractions;

    public class StateMachine<T> : IStateMachine<T> where T : Enum
    {
        public T State => _switchableStateMachine.State;
        public IStateSwitcher<T> StateSwitcher => _switchableStateMachine.StateSwitcher;

        public event StateEnterDelegate<T>? StateEnter
        {
            add => _switchableStateMachine.StateEnter += value;
            remove => _switchableStateMachine.StateEnter -= value;
        }
        public event StateExitDelegate<T>? StateExit
        {
            add => _switchableStateMachine.StateExit += value;
            remove => _switchableStateMachine.StateExit -= value;
        }

        public StateMachine(IStateGraph<T> stateGraph)
        {
            _switchableStateMachine = new SwitchableStateMachine(stateGraph);
        }

        private readonly SwitchableStateMachine _switchableStateMachine;

        private class SwitchableStateMachine : IStateMachine<T>, IStateSwitcher<T>
        {
            public T State => _state;
            public IStateSwitcher<T> StateSwitcher => this;

            public event StateEnterDelegate<T>? StateEnter;
            public event StateExitDelegate<T>? StateExit;

            public SwitchableStateMachine(IStateGraph<T> stateGraph)
            {
                _stateGraph = stateGraph;
                _state = _stateGraph.InitialState;
            }
            public IEnumerable<T> ValidStateTransitions(T state) => _stateGraph.Transitions[state];
            public bool IsCanTransitTo(T state) => _stateGraph.Transitions[_state].Contains(state);
            public void TransitTo(T state, object? context = null)
            {
                if (IsCanTransitTo(state))
                {
                    try
                    {
                        StateExit?.Invoke(_state, state);
                    }
                    finally
                    {
                        var fromState = _state;
                        _state = state;
                        StateEnter?.Invoke(fromState, _state, context);
                    }
                }
                else
                {
                    throw new InvalidStateMachineTransitionException<T>(_state, state);
                }
            }
            public bool TryTransitTo(T state, object? context = null)
            {
                if (IsCanTransitTo(state))
                {
                    try
                    {
                        StateExit?.Invoke(_state, state);
                    }
                    finally
                    {
                        var fromState = _state;
                        _state = state;
                        StateEnter?.Invoke(fromState, _state, context);
                    }
                    return true;
                }
                return false;
            }
            public void TransitToInitial()
            {
                if (!_state.Equals(_stateGraph.InitialState))
                {
                    StateExit?.Invoke(_state, _stateGraph.InitialState);
                }
                var fromState = _state;
                _state = _stateGraph.InitialState;
                StateEnter?.Invoke(fromState, _state, null);
            }

            private T _state;
            private readonly IStateGraph<T> _stateGraph;
        }
    }
}