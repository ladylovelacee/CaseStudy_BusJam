using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleManager : Singleton<VehicleManager>
    {
        [SerializeField]
        public Vector2[] targetPoints = new Vector2[3]; // Spawn, waiting, finish points
        [SerializeField]
        private VehicleBase vehiclePrefab;
        private VehicleSpawner _spawner;
        public VehicleBase CurrentVehicle { get; private set; }
        private void OnEnable()
        {
            LevelManager.Instance.OnLevelStarted += onLevelStarted;
        }

        private void OnDisable()
        {
            LevelManager.Instance.OnLevelStarted -= onLevelStarted;
        }

        private void onLevelStarted() //TODO: Move to on game start
        {
            _spawner = new(3, vehiclePrefab);
            _spawner.Spawn(ColorIDs.Green, targetPoints[0]);
        }

        private void OnDrawGizmos()
        {
            if (targetPoints == null || targetPoints.Length == 0) return;

            for (int i = 0; i < targetPoints.Length; i++)
            {
                if (targetPoints[i] == null) continue;
                
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPoints[i], 2f);

                if (i < targetPoints.Length - 1 && targetPoints[i + 1] != null)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(targetPoints[i], targetPoints[i + 1]);
                }
            }
        }
    }

    public class  VehicleSpawner
    {
        private int _totalVehicle;
        private VehicleBase _vehicle;

        public VehicleSpawner(int totalVehicle, VehicleBase vehicle)
        {
            _totalVehicle = totalVehicle;
            _vehicle = vehicle;
        }

        public void Spawn(ColorIDs color, Vector3 spawnPoint)
        {
            VehicleBase instance = MonoBehaviour.Instantiate(_vehicle, spawnPoint, Quaternion.identity); // TODO: Get from pool
            instance.Initialize(color);
        }
    }
}