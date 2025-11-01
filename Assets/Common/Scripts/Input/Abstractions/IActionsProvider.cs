#nullable enable

using System;

namespace KarenKrill.Input.Abstractions
{
    public interface IActionsProvider<ActionMapEnumType> where ActionMapEnumType : Enum
    {
        public event Action<ActionMapEnumType>? ActionMapChanged;

        public void SetActionMap(ActionMapEnumType actionMap);
        public void Disable();
    }
}
