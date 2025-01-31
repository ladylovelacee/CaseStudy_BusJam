using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int width, height;
        public List<StickmanData> stickmen = new();
        public List<CabinData> cabins = new();
        public List<Vector2Int> obstacles = new();
        public List<VehicleData> busQueue = new();
    }

    [System.Serializable]
    public class StickmanData
    {
        public Vector2Int position;
        public ColorIDs stickmanColor;
        public string ability;
    }

    [System.Serializable]
    public class CabinData
    {
        public Vector2Int position;
        public List<Color> containedColors;
    }

    [System.Serializable]
    public class VehicleData
    {
        public ColorIDs colorId;
        public int arrivalOrder;
    }
}