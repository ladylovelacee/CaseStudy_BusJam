using Runtime.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleManager : Singleton<VehicleManager>
    {
        [field:SerializeField] public Transform WaitPoint { get; private set; }
        [field:SerializeField] public Transform FinishPoint { get; private set; }

        public LevelData levelData; // TODO: Get level data from level manager
        public VehicleBase CurrentVehicle { get; private set; }
        
        [SerializeField] private Transform spawnPoint;

        private VehicleBase vehicleInstance => DataManager.Instance.InstanceContainer.Vehicle;
        private Queue<VehicleData> busQueue = new Queue<VehicleData>();

        private void Start()
        {
            InitializeBusQueue();
            onLevelStarted();
        }

        private void InitializeBusQueue()
        {
            foreach (VehicleData bus in levelData.busQueue)
            {
                busQueue.Enqueue(bus);
            }
        }

        private void onLevelStarted() //TODO: Move to on game start
        {
            SpawnNextVehicle();
        }

        private void SpawnNextVehicle()
        {
            if (busQueue.Count > 0)
            {
                VehicleData nextBus = busQueue.Dequeue();
                CurrentVehicle = Instantiate(vehicleInstance, spawnPoint.position, Quaternion.identity);
                CurrentVehicle.Initialize(nextBus.colorId);
            }
        }

        public bool CanPassengerBoard(PassengerBase passenger)=> passenger._colorId.Equals(CurrentVehicle.ColorID) && !CurrentVehicle.IsFull;
        public void OnBusFilled()
        {
            if (CurrentVehicle != null)
            {
                CurrentVehicle = null;
                SpawnNextVehicle();
            }
        }
    }
}