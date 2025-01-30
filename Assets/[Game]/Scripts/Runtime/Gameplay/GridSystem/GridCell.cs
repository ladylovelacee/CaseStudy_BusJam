using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridCell : MonoBehaviour 
    {
        public int x, y;
        public int index;
        public int gCost, hCost, fCost;
        public int cameFromNodeIndex;

        public bool isWalkable;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }
    }
}