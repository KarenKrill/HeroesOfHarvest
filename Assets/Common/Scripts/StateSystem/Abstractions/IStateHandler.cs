#nullable enable

using System;

namespace KarenKrill.StateSystem.Abstractions
{
    public interface IStateHandler<T> where T : Enum
    {
        /// <summary>
        /// Processable state
        /// </summary>
        public T State { get; }

        void Enter(T prevState, object? context);
        void Exit(T nextState);
    }
}