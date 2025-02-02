using DG.Tweening;
using Runtime.Core;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleBase : MonoBehaviour
    {
        [SerializeField] Renderer m_Renderer;
        [SerializeField] DummyPassenger[] m_DummyPassengerArray;
        public ColorIDs ColorID { get; private set; }
        public bool IsFull => currentPassengers >= maxCapacity;
        public bool IsBoarded;
        private VehicleManager Manager => VehicleManager.Instance;

        private int maxCapacity = 3; // TODO: make generic
        public int currentPassengers = 0;
        private int _dummyPassengerIndex = 0;
        private Tween _checkTween;

        private const float MoveDuration = 1.5f;
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
            for (int i = 0; i < m_DummyPassengerArray.Length; i++)
            {
                m_DummyPassengerArray[i].gameObject.SetActive(false);
            }
        }

        public void Initialize(ColorIDs color)
        {
            currentPassengers = 0;
            _dummyPassengerIndex = 0;
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
            transform.DOMove(target, MoveDuration).SetEase(Ease.InSine)
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

            for (int i = 0; i < m_DummyPassengerArray.Length; i++)
            {
                m_DummyPassengerArray[i].SetColor(color);
                m_DummyPassengerArray[i].gameObject.SetActive(false);
            }
        }

        public int GetCurrentPassengerCount() => currentPassengers;

        public void AddPassenger(float checkDelay)
        {
            m_DummyPassengerArray[_dummyPassengerIndex].gameObject.SetActive(true);
            _dummyPassengerIndex++;
            if (_dummyPassengerIndex >= m_DummyPassengerArray.Length)
            {
                _checkTween?.Kill();
                _checkTween = DOVirtual.DelayedCall(checkDelay != 0 ? .25f : 0, () =>
                {
                    Move(Manager.FinishPoint.position);
                    Manager.OnVehicleFilled();
                }, false);
            }
        }
    }
}