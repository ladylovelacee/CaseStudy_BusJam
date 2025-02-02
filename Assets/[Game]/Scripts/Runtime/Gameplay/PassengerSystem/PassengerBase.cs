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
        private PassengerVisual _passengerVisual;

        #region Methods From MonoBehaviour
        private void Awake()
        {
            _passengerVisual = new(this);
        }

        private void OnEnable()
        {
            LevelManager.Instance.LevelLoader.OnLevelStartLoading += onLevelStartedLoading;
        }

        private void OnDisable()
        {
            SetPassengerSelectable(false);
            LevelManager.Instance.LevelLoader.OnLevelStartLoading -= onLevelStartedLoading;
        }
        #endregion

        private void onLevelStartedLoading()
        {
            Manager.PassengerPool.Release(this);
        }

        public void SetStickmanData(StickmanData data)
        {
            Data = data;
            _passengerVisual.SetColor(DataManager.Instance.ColorContainer.GetColorById(data.stickmanColor));
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
            _passengerVisual.SetOutline(state);
        }
    }

    public struct PassengerVisual
    {
        private PassengerBase _base;
        public PassengerVisual(PassengerBase passenger)
        {
            _base = passenger;
        }

        public void SetColor(Color color) 
        {
            MaterialPropertyBlock block = new();
            block.SetColor("_Color", color);
            _base.Renderer.SetPropertyBlock(block);
        }

        public void SetOutline(bool isActive)
        {
            // TODO: Outline process
        }
    }
}