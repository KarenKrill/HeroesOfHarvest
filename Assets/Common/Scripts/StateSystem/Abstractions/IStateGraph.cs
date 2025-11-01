using System;
using System.Collections.Generic;

namespace KarenKrill.StateSystem.Abstractions
{
    public interface IStateGraph<T> where T : Enum
    {
        T InitialState { get; }
        IDictionary<T, IList<T>> Transitions { get; }
    }
}
