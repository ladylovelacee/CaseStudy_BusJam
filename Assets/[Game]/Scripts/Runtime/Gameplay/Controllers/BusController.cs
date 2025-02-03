using DG.Tweening;
using Runtime.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class BusController : IController
    {
        public Bus CurrentBus => busQueue.Peek();
        private LevelManager LevelManager => LevelManager.Instance;
        private ObjectPoolBase<Bus> BusPool => _gameplayArea.BusPool;

        private GameplayArea _gameplayArea;

        private Queue<VehicleData> busDataQueue = new Queue<VehicleData>();
        private Queue<Bus> busQueue = new Queue<Bus>();

        private int _boardedCount = 0;
        private int _spawnedBusCount = 0;

        private bool _isBusy;

        private const int Capasity = 3;
        private const float Distance = 10f;
        private const float BusSpeed = 10f;
        private Tween parentMoveTween;
        public BusController(GameplayArea gameplayArea)
        {
            _gameplayArea = gameplayArea;
        }

        #region Methods From Interfaces
        public void Dispose()
        {

        }

        public void Initialize()
        {
            List<VehicleData> datas = new(_gameplayArea.levelData.busQueue);
            foreach (VehicleData bus in datas)
            {
                busDataQueue.Enqueue(bus);

                SpawnBus(bus);
            }

        }
        
        public void Reset()
        {
            busDataQueue.Clear();
            busQueue.Clear();
            _boardedCount = 0;
            
            DOTween.Complete(parentMoveTween);
            _gameplayArea.BusPool.Release(CurrentBus);
        }
        #endregion

        public bool BoardToCurrentBus(StickmanGridObj stickman)
        {
            if (_isBusy) return false;
            if(_boardedCount < Capasity && stickman.Data.stickmanColor == CurrentBus.busColor)
            {
                _boardedCount++;
                if (_boardedCount == Capasity)
                    HandleBusFilled();

                return true;
            }
            else
                return false;
        }

        private void HandleBusFilled()
        {
            if(_isBusy) return;
            _isBusy = true;
            // TODO: wait until last passenger arrive
            Transform parent = _gameplayArea.BusQueueParent.transform;
            float newPos = parent.transform.position.x + Distance;
            parentMoveTween = parent.DOMoveX(newPos, BusSpeed).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    if (!_isBusy) return;

                    BusPool.Release(CurrentBus);
                    busQueue.Dequeue();
                    _boardedCount = 0;

                    if(busQueue.Count == 0)
                        LevelManager.CompleteLevel(true);

                    _isBusy = false;
                });
        }

        private void SpawnBus(VehicleData data)
        {
            Bus bus = _gameplayArea.BusPool.Get();
            bus.transform.position = _gameplayArea.BusQueueParent.transform.position  - new Vector3(_spawnedBusCount * Distance, 0, 0);
            bus.Initialize(data.colorId);
        }
    }
}
