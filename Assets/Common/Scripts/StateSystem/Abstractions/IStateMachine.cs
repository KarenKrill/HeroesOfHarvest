#nullable enable

using System;

namespace KarenKrill.StateSystem.Abstractions
{
    public delegate void StateEnterDelegate<T>(T fromState, T toState, object? context) where T : Enum;
    public delegate void StateExitDelegate<T>(T fromState, T toState) where T : Enum;

    public interface IStateMachine<T> where T : Enum
    {
        T State { get; }
        IStateSwitcher<T> StateSwitcher { get; }


        public event StateEnterDelegate<T>? StateEnter;

        public event StateExitDelegate<T>? StateExit;
    }
}