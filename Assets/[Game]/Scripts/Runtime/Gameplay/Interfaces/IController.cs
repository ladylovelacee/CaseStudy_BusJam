namespace Runtime.Gameplay
{
    public interface IController
    {
        void Initialize(); // Call on Level loaded
        void Reset(); // Call on start loading
        void Dispose(); // Call on destroy
    }
}
