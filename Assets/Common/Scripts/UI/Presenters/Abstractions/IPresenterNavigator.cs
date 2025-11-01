namespace KarenKrill.UI.Presenters.Abstractions
{
    public interface IPresenterNavigator
    {
        void Push(IPresenter presenter);
        void Pop();
        void Clear();
    }
}
