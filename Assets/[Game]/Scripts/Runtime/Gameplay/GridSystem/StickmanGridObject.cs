using UnityEngine;

namespace Runtime.Gameplay
{
    public class StickmanGridObj : GridObject, ISelectable
    {
        #region Properties
        private bool _isSelectable;
        public bool IsSelectable { get => _isSelectable; set => SetIsSelectable(value); }

        public StickmanData Data { get; private set; }
        #endregion

        public Renderer Renderer;
        public Vector2Int ExitPoint;

        private StickmanVisual _visual;

        public void Initialize(StickmanData data)
        {
            _visual = new(this);
            Data = data;

            LoadData();
        }

        private void LoadData()
        {
            _visual.SetColor(DataManager.Instance.ColorContainer.GetColorById(Data.stickmanColor));
            Position = Data.position;
        }

        private void SetIsSelectable(bool isSelectable)
        {
            _isSelectable = isSelectable;
            // TODO: Outline
        }

        public void Select()
        {
            if (_isSelectable) return;
            SetIsSelectable(false);
        }

        public struct StickmanVisual
        {
            private StickmanGridObj _obj;
            public StickmanVisual(StickmanGridObj stickman)
            {
                _obj = stickman;
            }

            public void SetColor(Color color)
            {
                MaterialPropertyBlock block = new();
                block.SetColor("_BaseColor", color);
                _obj.Renderer.SetPropertyBlock(block);
            }

            public void SetOutline(bool isActive)
            {
                // TODO: Outline process
            }
        }
    }    
}