using DG.Tweening;
using Runtime.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleManager : Singleton<VehicleManager>
    {
        public event Action OnVehicleBoarded;

        [field:SerializeField] public Transform WaitPoint { get; private set; }
        [field:SerializeField] public Transform FinishPoint { get; private set; }
        public VehicleBase CurrentVehicle { get; private set; }
        
        [SerializeField] private Transform spawnPoint;

        private VehicleBase vehicleInstance => DataManager.Instance.InstanceContainer.Vehicle;
        public Queue<VehicleData> busQueue { get; private set; } = new Queue<VehicleData>();
        private LevelData levelData => LevelLoader.CurrentLevelData;
        public ObjectPoolBase<VehicleBase> Pool { get; private set; }

        private void Awake()
        {
            Pool = new(vehicleInstance);
        }

        public void Initialize()
        {
            if(CurrentVehicle != null)
            {
                Pool.Release(CurrentVehicle);
                DOTween.KillAll(CurrentVehicle.gameObject);
            }

            List<VehicleData> vehicleDatas = new();
            if (GameplaySaveSystem.CurrentSaveData != null)
                vehicleDatas = GameplaySaveSystem.CurrentSaveData.BusQueue;           
            else
                vehicleDatas = levelData.busQueue;

            foreach (VehicleData bus in vehicleDatas)
            {
                busQueue.Enqueue(bus);
            }
            SpawnFirstVehicle();
        }

        private void SpawnFirstVehicle()
        {
            SpawnNextVehicle(true);
            if (GameplaySaveSystem.CurrentSaveData != null)
            {
                for (int i = 0; GameplaySaveSystem.CurrentSaveData.LastBusPassengerCount>i; i++)
                {
                    CurrentVehicle.currentPassengers++;
                    CurrentVehicle.AddPassenger(0);
                }
            }
            CurrentVehicle.transform.position = WaitPoint.position;
        }

        private void SpawnNextVehicle(bool isFirstBus)
        {
            if (busQueue.Count > 0)
            {
                VehicleData nextBus = busQueue.Peek();

                CurrentVehicle = Pool.Get();
                CurrentVehicle.transform.position = spawnPoint.position;
                CurrentVehicle.Initialize(nextBus.colorId, isFirstBus);
            }
        }

        public bool CanPassengerBoard(PassengerBase passenger)=> passenger.Data.stickmanColor.Equals(CurrentVehicle.ColorID) && !CurrentVehicle.IsFull && CurrentVehicle. IsBoarded;
        public void OnVehicleFilled()
        {
            if (CurrentVehicle != null)
            {
                busQueue.Dequeue();
                CurrentVehicle = null;
                SpawnNextVehicle(false);
            }

            if (busQueue.Count <= 0)
                LevelManager.Instance.CompleteLevel(true);
        }

        public void OnVehicleWaitForPassengers()
        {
            OnVehicleBoarded?.Invoke();
        }
    }
}