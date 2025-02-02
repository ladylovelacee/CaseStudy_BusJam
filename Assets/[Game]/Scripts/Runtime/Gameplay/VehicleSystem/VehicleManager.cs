using Runtime.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleManager : Singleton<VehicleManager>
    {
        [field:SerializeField] public Transform WaitPoint { get; private set; }
        [field:SerializeField] public Transform FinishPoint { get; private set; }
        public VehicleBase CurrentVehicle { get; private set; }
        
        [SerializeField] private Transform spawnPoint;

        private VehicleBase vehicleInstance => DataManager.Instance.InstanceContainer.Vehicle;
        public Queue<VehicleData> busQueue { get; private set; } = new Queue<VehicleData>();

        public void Initialize(LevelData data)
        {
            foreach (VehicleData bus in data.busQueue)
            {
                busQueue.Enqueue(bus);
            }

            SpawnNextVehicle();
        }

        private void SpawnNextVehicle()
        {
            if (busQueue.Count > 0)
            {
                VehicleData nextBus = busQueue.Peek();
                CurrentVehicle = Instantiate(vehicleInstance, spawnPoint.position, Quaternion.identity);
                CurrentVehicle.Initialize(nextBus.colorId);
            }
        }

        public bool CanPassengerBoard(PassengerBase passenger)=> passenger._colorId.Equals(CurrentVehicle.ColorID) && !CurrentVehicle.IsFull;
        public void OnBusFilled()
        {
            if (CurrentVehicle != null)
            {
                busQueue.Dequeue();
                CurrentVehicle = null;
                SpawnNextVehicle();
            }
        }
    }
}