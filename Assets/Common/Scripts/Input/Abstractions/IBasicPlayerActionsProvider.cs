#nullable enable

using System;

using UnityEngine;

namespace KarenKrill.Input.Abstractions
{
    public delegate void MoveDelegate(Vector2 moveDelta);
    public delegate void LookDelegate(Vector2 lookDelta);

    public interface IBasicPlayerActionsProvider
    {
        public Vector2 LastLookDelta { get; }
        public Vector2 LastMoveDelta { get; }
        public bool IsSprintActive { get; }
        public bool IsCrouchActive { get; }
        public bool IsJumpActive { get; }
        public bool IsAttackActive { get; }
        public bool IsInteractActive { get; }

        public event LookDelegate? Look;
        public event Action? LookCancel;
        public event Action? MoveStarted;
        public event MoveDelegate? Move;
        public event Action? MoveCancel;
        public event Action? Sprint;
        public event Action? SprintCancel;
        public event Action? Crouch;
        public event Action? CrouchCancel;
        public event Action? Jump;
        public event Action? JumpCancel;
        public event Action? Attack;
        public event Action? AttackCancel;
        public event Action? Interact;
        public event Action? InteractCancel;
        public event Action? Pause;
    }
}
