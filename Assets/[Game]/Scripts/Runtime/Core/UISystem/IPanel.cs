namespace Runtime.Core
{
    public interface IPanel
    {
        void Initialize();
        void Dispose();
        void OpenPanel();
        void ClosePanel();
    }
}