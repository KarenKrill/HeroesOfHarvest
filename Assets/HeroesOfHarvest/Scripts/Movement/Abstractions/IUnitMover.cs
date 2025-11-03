#nullable enable

using System;
using UnityEngine;

namespace HeroesOfHarvest
{
    public interface IUnitMover
    {
        event Action? PathCancelled;
        event Action? PathCompleted;

        float CompletionMonitorPeriodTime { get; set; }

        void GoTo(Vector3 target);
    }
}
