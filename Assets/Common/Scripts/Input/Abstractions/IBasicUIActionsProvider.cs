#nullable enable

using System;

using UnityEngine;

namespace KarenKrill.Input.Abstractions
{
    public delegate void NavigateDelegate(Vector2 value);
    public delegate void PointDelegate(Vector2 value);
    public delegate void ScrollWheelDelegate(Vector2 value);
    public delegate void TrackedDevicePositionDelegate(Vector3 value);
    public delegate void TrackedDeviceOrientationDelegate(Quaternion value);

    public interface IBasicUIActionsProvider
    {
        public Vector2 LastNavigateValue { get; }
        public Vector2 LastPointValue { get; }
        public Vector2 LastScrollWheelValue { get; }
        public Vector3 LastTrackedDevicePosition { get; }
        public Quaternion LastTrackedDeviceOrientation { get; }

        public event NavigateDelegate? Navigate;
        public event Action? NavigateCancel;
        public event PointDelegate? Point;
        public event Action? PointCancel;
        public event ScrollWheelDelegate? ScrollWheel;
        public event Action? ScrollWheelCancel;
        public event TrackedDevicePositionDelegate? TrackedDevicePosition;
        public event Action? TrackedDevicePositionCancel;
        public event TrackedDeviceOrientationDelegate? TrackedDeviceOrientation;
        public event Action? TrackedDeviceOrientationCancel;
        public event Action? Submit;
        public event Action? Cancel;
        public event Action? Click;
        public event Action? RightClick;
        public event Action? MiddleClick;
    }
}
