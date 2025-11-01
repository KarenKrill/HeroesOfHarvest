#nullable enable

using System;

namespace KarenKrill.GameFlow.Abstractions
{
    public interface ITimeLimitedLevelInfoProvider : ILevelInfoProvider
    {
        float MaxCompleteTime { get; }
        float RemainingTime { get; }
        float WarningTime { get; }
        float LastWarningTime { get; }

        event Action<float>? MaxCompleteTimeChanged;
        event Action<float>? RemainingTimeChanged;
        event Action<float>? WarningTimeChanged;
        event Action<float>? LastWarningTimeChanged;
    }
}