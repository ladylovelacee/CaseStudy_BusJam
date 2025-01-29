using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerBase : MonoBehaviour
    {
        [field: SerializeField] public Renderer Renderer { get; private set; }
        public PassengerColor PassengerColor { get; private set; }

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
    }

    public struct PassengerColor
    {
        private PassengerBase _base;
        public PassengerColor(PassengerBase renderer)
        {
            _base = renderer;
        }

        public void SetColor(Color color) 
        {
            MaterialPropertyBlock block = new();
            block.SetColor("_Color", color);
            _base.Renderer.SetPropertyBlock(block);
        }
    }
}