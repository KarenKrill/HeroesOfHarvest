namespace KarenKrill.Input.Abstractions
{
    public enum ActionMap
    {
        Player,
        UI
    }
    public interface IBasicActionsProvider : IActionsProvider<ActionMap>
    {
    }
}
