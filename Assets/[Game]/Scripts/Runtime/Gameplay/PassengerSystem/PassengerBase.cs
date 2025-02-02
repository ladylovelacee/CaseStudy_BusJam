using DG.Tweening;
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
            DOTween.Kill(gameObject);

            SetPassengerSelectable(false);
            VehicleManager.Instance.OnVehicleBoarded -= onVehicleBoarded;

            LevelManager.Instance.LevelLoader.OnLevelStartLoading -= onLevelStartedLoading;
        }
        #endregion

        public void CheckCurrentBus()
        {
            LevelManager.Instance.OnLevelStarted += onLevelStarted;
        }
        private void onLevelStarted()
        {
            LevelManager.Instance.OnLevelStarted -= onLevelStarted;
            onVehicleBoarded();
        }

        private void onLevelStartedLoading()
        {
            DOTween.Kill(gameObject);
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

        public void StartPeeking()
        {
            VehicleManager.Instance.OnVehicleBoarded += onVehicleBoarded;
        }

        public void onVehicleBoarded()
        {
            if (Manager.CanBoardVehicle(this))
            {
                VehicleManager.Instance.OnVehicleBoarded -= onVehicleBoarded;
                WaitingAreaManager.Instance._currentAvailableSlotCount++;
                VehicleManager.Instance.CurrentVehicle.currentPassengers++;

                transform.DOMove(VehicleManager.Instance.CurrentVehicle.transform.position, .5f).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        VehicleManager.Instance.CurrentVehicle.AddPassenger(.5f);
                        Manager.PassengerPool.Release(this);
                    }).SetLink(gameObject);
            }
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
            block.SetColor("_BaseColor", color);
            _base.Renderer.SetPropertyBlock(block);
        }

        public void SetOutline(bool isActive)
        {
            // TODO: Outline process
        }
    }
}