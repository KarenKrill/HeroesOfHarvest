namespace KarenKrill.UI.Views.Abstractions
{
    public interface IView
    {
        bool Interactable { get; set; }
        void Show();
        void Close();
        void SetFocus(bool isFocused);
    }
}