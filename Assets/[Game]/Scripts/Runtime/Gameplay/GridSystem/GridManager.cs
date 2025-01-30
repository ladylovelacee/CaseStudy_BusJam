using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridManager : Singleton<GridManager>
    {
        public GridSystem<GridCell> Board { get; private set; }
        private float cellSize = 1;
        private Vector3 originPosition = Vector3.zero;
        private int width = 10, height = 10;
        private void Awake()
        {
            Initialize();
        }
        public void Initialize()
        {
            Board = new(width, height, 1, Vector3.zero, (int x, int y) => CreateCell(x,y)); // TODO: Pool integration

        }

        public void GetGridPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * cellSize + originPosition;
        }

        private GridCell CreateCell(int x, int y)
        {
            GridCell cell = Instantiate(DataManager.Instance.InstanceContainer.Cell, GetWorldPosition(x, y), Quaternion.identity);
            cell.x = x;
            cell.y = y;

            return cell;
        }
    }
}