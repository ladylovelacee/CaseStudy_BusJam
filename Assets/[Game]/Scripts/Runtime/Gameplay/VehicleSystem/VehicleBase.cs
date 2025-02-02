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
        
        private VehicleManager Manager => VehicleManager.Instance;

        private int maxCapacity = 3; // TODO: make generic
        private int currentPassengers = 0;

        private const float MoveDuration = 2.0f;
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
            ColorID = color;
            VisualSetup();
            Move(Manager.WaitPoint.position);
        }
        private void Move(Vector3 target)
        {
            transform.DOMove(target, MoveDuration);
        }
        
        private void VisualSetup()
        {
            Color color = DataManager.Instance.ColorContainer.GetColorById(ColorID);
            MaterialPropertyBlock block = new();
            block.SetColor("_Color", color);
            m_Renderer.SetPropertyBlock(block);
        }

        public int GetCurrentPassengerCount() => currentPassengers;
        public void AddPassenger()
        {
            currentPassengers++;

            if (IsFull)
            {
                Move(Manager.FinishPoint.position);
                Manager.OnBusFilled();
            }
        }
    }
}