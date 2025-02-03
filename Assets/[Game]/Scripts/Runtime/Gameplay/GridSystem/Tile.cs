using UnityEngine;

namespace Runtime.Gameplay
{
    public class Tile : MonoBehaviour
    {
        [HideInInspector] public Vector2Int Position;
        [HideInInspector] public int Index;

        [HideInInspector] public GridObject GridObject;
        [HideInInspector] public bool IsWalkable;

        public void Init(Vector2Int pos, int index, bool isWalkable)
        {
            Position = pos;
            Index = index;
            IsWalkable = isWalkable;

            GridObject = null;
        }

        public bool HasObject()
        {
            return GridObject != null;
        }

        public void SetGridObject(GridObject obj)
        {
            GridObject = obj;
            IsWalkable = false;
        }

        public void ClearTileObject()
        {
            GridObject = null;
            IsWalkable = true;
        }
    }
}