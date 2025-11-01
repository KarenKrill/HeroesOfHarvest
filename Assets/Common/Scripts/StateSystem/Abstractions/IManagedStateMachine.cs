using System;

namespace KarenKrill.StateSystem.Abstractions
{
    public interface IManagedStateMachine<T> where T : Enum
    {
        IStateMachine<T> StateMachine { get; }

        void AddStateHandler(IStateHandler<T> stateHandler);
        void RemoveStateHandler(T state);
        void Start();
    }
}
