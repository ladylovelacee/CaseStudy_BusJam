using Runtime.Core;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerBase : MonoBehaviour, ISelectable
    {
        #region Properties
        [field: SerializeField] public Renderer Renderer { get; private set; }
        public bool IsSelectable { get; private set; } = false;
        public StickmanData Data { get; private set; }
        private PassengerManager Manager => PassengerManager.Instance;
        #endregion

        public Vector2Int TargetBoardPos;
        private PassengerColor _passengerColor;

        private void Awake()
        {
            _passengerColor = new(this);
        }

        private void OnEnable()
        {
            LevelManager.Instance.LevelLoader.OnLevelStartLoading += onLevelStartedLoading;    
        }

        private void OnDisable()
        {
            IsSelectable = false;
            LevelManager.Instance.LevelLoader.OnLevelStartLoading -= onLevelStartedLoading;
        }

        private void onLevelStartedLoading()
        {
            Manager.PassengerPool.Release(this);
        }

        public void SetStickmanData(StickmanData data)
        {
            Data = data;
            _passengerColor.SetColor(DataManager.Instance.ColorContainer.GetColorById(data.stickmanColor));
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