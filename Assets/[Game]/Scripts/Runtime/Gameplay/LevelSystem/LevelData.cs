using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int duration;
        public int width, height;
        public List<StickmanData> stickmen = new();
        public List<Vector2Int> obstacles = new();
        public List<VehicleData> busQueue = new();
        
        public static LevelData CopyLevelData(LevelData source)
        {
            LevelData copy = ScriptableObject.CreateInstance<LevelData>();

            // Copy primitive values directly
            copy.width = source.width;
            copy.height = source.height;

            copy.stickmen = new List<StickmanData>();
            foreach (var stickman in source.stickmen)
            {
                var newStickman = new StickmanData
                {
                    position = stickman.position,
                    worldPosition = stickman.worldPosition,
                    stickmanColor = stickman.stickmanColor
                };
                copy.stickmen.Add(newStickman);
            }

            copy.obstacles = new List<Vector2Int>(source.obstacles); 

            copy.busQueue = new List<VehicleData>();
            foreach (var vehicle in source.busQueue)
            {
                var newVehicle = new VehicleData
                {
                    colorId = vehicle.colorId,
                    arrivalOrder = vehicle.arrivalOrder
                };
                copy.busQueue.Add(newVehicle);
            }

            return copy;
        }

        public void ResetLevelData()
        {
            width = 0; height = 0;
            stickmen.Clear();
            obstacles.Clear();
            busQueue.Clear();
        }
    }

    [System.Serializable]
    public class StickmanData
    {
        public Vector2Int position;
        public Vector3 worldPosition;
        public ColorIDs stickmanColor;
    }

    [System.Serializable]
    public class VehicleData
    {
        public ColorIDs colorId;
        public int arrivalOrder;
    }
}