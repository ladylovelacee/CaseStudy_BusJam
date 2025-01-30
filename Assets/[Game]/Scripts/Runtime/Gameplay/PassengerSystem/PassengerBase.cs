using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerBase : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public Renderer Renderer { get; private set; }
        public PassengerColor PassengerColor { get; private set; }

        public bool IsSelectable { get; private set; } = false;

        private ColorIDs _colorId;

        private void Awake()
        {
            PassengerColor = new(this);
        }

        public void Initialize(ColorIDs colorId)
        {
            _colorId = colorId;
            PassengerColor.SetColor(DataManager.Instance.ColorContainer.GetColorById(colorId));
        }

        public void Select()
        {
            Debug.Log("Selected");
            PassengerManager.Instance.HandlePassengerSelection(this);
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