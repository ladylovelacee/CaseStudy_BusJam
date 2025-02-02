using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerBase : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public Renderer Renderer { get; private set; }
        public PassengerColor PassengerColor { get; private set; }
        public Vector2Int Position { get; private set; }

        public bool IsSelectable { get; private set; } = false;
        public Vector2Int TargetPos;
        private PassengerManager Manager => PassengerManager.Instance;
        public StickmanData Data { get; private set; }

        public ColorIDs _colorId;

        private void Awake()
        {
            PassengerColor = new(this);
        }

        public void SetStickmanData(StickmanData data)
        {
            Data = data;
            Initialize(data.stickmanColor, data.position);
        }

        private void Initialize(ColorIDs colorId, Vector2Int pos)
        {
            Position = pos;
            _colorId = colorId;
            PassengerColor.SetColor(DataManager.Instance.ColorContainer.GetColorById(colorId));
        }

        public void Select()
        {
            SetPassengerSelectable(false);
            Manager.RemovePassenger(this);

            Manager.HandlePassengerSelection(this);
        }

        public void SetPassengerSelectable(bool state)
        {
            IsSelectable = state;
            // TODO: Outline process
        }
    }

    public interface ISelectable
    {
        bool IsSelectable {  get; }
        void Select();
    }

    public struct PassengerColor
    {
        private PassengerBase _base;
        public PassengerColor(PassengerBase passenger)
        {
            _base = passenger;
        }

        public void SetColor(Color color) 
        {
            MaterialPropertyBlock block = new();
            block.SetColor("_Color", color);
            _base.Renderer.SetPropertyBlock(block);
        }
    }
}