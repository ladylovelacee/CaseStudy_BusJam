using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridCell : MonoBehaviour 
    {
        public int x, y;
        public bool IsWalkable { get; private set; }
    }
}