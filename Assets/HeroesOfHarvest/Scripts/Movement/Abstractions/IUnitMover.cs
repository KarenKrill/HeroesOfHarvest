#nullable enable

using System;
using UnityEngine;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.Movement
{
    public interface IUnitMover
    {
        event Action<IUnit>? UnitPathCancelled;
        event Action<IUnit>? UnitPathCompleted;

        float CompletionMonitorPeriodTime { get; set; }

        bool TrySend(IUnit unit, Vector3 position);
    }
}
