#nullable enable

using System;
using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;

namespace HeroesOfHarvest.Input.Abstractions
{
    public delegate void InteractPointDelegate(Vector2 pointDelta);
    public interface IStrategyGameActionsProvider : IBasicActionsProvider
    {
        Vector2 LastCameraLookDelta { get; }
        Vector2 LastCameraMoveDelta { get; }
        Vector2 LastPointDelta { get; }

        event LookDelegate? CameraLook;
        event Action? CameraLookCancel;
        event Action? CameraMoveStarted;
        event MoveDelegate? CameraMove;
        event Action? CameraMoveCancel;
        event InteractPointDelegate? InteractPoint;
        event Action? InteractClickDown;
        event Action? InteractClickUp;
    }
}
