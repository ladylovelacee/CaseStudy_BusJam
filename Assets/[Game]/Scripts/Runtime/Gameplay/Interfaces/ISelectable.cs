namespace Runtime.Gameplay
{
    public interface ISelectable
    {
        bool IsSelectable { get; }
        void Select();
    }
}