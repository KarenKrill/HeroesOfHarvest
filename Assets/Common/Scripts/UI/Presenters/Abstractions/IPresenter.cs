namespace KarenKrill.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public interface IPresenter
    {
        void Enable();
        void Disable();
    }
    public interface IPresenter<T> : IPresenter where T : IView { }
}