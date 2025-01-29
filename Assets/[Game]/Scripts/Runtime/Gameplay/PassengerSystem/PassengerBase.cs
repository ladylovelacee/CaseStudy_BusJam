using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerBase : MonoBehaviour
    {
        [field: SerializeField] public Renderer m_Renderer { get; private set; }
        public PassengerColor PassengerColor { get; private set; }

        private ColorIDs _colorId;

        private void Awake()
        {
            PassengerColor = new(this);
        }

        public void Initialize(ColorIDs colorId)
        {
            _colorId = colorId;
            //PassengerColor.SetColor();
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
            _base.m_Renderer.SetPropertyBlock(block);
        }
    }
}