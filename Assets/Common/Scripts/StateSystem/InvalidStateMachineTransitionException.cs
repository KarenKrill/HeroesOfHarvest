using System;

namespace KarenKrill.StateSystem
{
    public class InvalidStateMachineTransitionException<T> : Exception
    {
        public T FromState { get; }
        public T ToState { get; }

        public InvalidStateMachineTransitionException(T fromState, T toState) :
            base(string.Format("Invalid state transition {0} -> {1}", fromState, toState))
        {
            FromState = fromState;
            ToState = toState;
        }
    }
}
