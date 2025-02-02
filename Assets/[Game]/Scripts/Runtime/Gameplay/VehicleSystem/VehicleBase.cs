using DG.Tweening;
using Runtime.Core;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleBase : MonoBehaviour
    {
        [SerializeField] Renderer m_Renderer;
        public ColorIDs ColorID { get; private set; }
        public bool IsFull => currentPassengers >= maxCapacity;
        public bool IsBoarded;
        private VehicleManager Manager => VehicleManager.Instance;

        private int maxCapacity = 3; // TODO: make generic
        public int currentPassengers = 0;

        private const float MoveDuration = 1;
        private void OnEnable()
        {
            LevelManager.Instance.LevelLoader.OnLevelStartLoading += onLevelStartLoading;
        }
        private void OnDisable()
        {
            LevelManager.Instance.LevelLoader.OnLevelStartLoading -= onLevelStartLoading;
        }

        private void onLevelStartLoading()
        {
            Manager.Pool.Release(this);
        }

        public void Initialize(ColorIDs color)
        {
            currentPassengers = 0;
            ColorID = color;
            VisualSetup();
            Move(Manager.WaitPoint.position, MoveDuration, onVehicleBoarded);
        }

        private void onVehicleBoarded()
        {
            IsBoarded = true;
            Manager.OnVehicleWaitForPassengers();
        }
        private void Move(Vector3 target, float duration = 1, Action stopAction = null)
        {
            transform.DOMove(target, MoveDuration)
                .OnComplete(()=>{
                    if(stopAction != null)
                        stopAction();
            }).SetLink(gameObject);
        }
        
        private void VisualSetup()
        {
            Color color = DataManager.Instance.ColorContainer.GetColorById(ColorID);
            MaterialPropertyBlock block = new();
            block.SetColor("_Color", color);
            m_Renderer.SetPropertyBlock(block, 0);
        }

        public int GetCurrentPassengerCount() => currentPassengers;
        public void AddPassenger()
        {
            if (IsFull)
            {
                Move(Manager.FinishPoint.position);
                Manager.OnVehicleFilled();
            }
        }
    }
}