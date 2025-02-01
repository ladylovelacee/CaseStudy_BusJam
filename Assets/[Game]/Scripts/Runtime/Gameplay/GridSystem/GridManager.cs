using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridManager : Singleton<GridManager>
    {
        public ObjectPoolBase<GridCell> CellPool {  get; private set; }
        public GridSystem<GridCell> Board { get; private set; }

        private float cellSize = 1;
        private Vector3 originPosition;
        public int width, height;

        [HideInInspector]
        public int[] walkableArea;
        private void Awake()
        {
            CellPool = new(DataManager.Instance.InstanceContainer.Cell);
        }

        public void Initialize(LevelData data)
        {
            width = data.width; 
            height = data.height;
            
            originPosition = new Vector3(-width/2f, 0, -height/2f);
            walkableArea = new int[width * height];
            Board = new(width, height, originPosition, (int x, int y) => CreateCell(x,y));
        }

        public Vector3 GetWorldPosition(int x, int y) => new Vector3(x, 0, y) * cellSize + originPosition;

        public void SetCellWalkable(int x, int y, bool isWalkable)
        {
            int index = Board.GetIndex(x, y);
            walkableArea[index] = isWalkable ? 1 : 0;
        }

        private GridCell CreateCell(int x, int y)
        {
            GridCell cell = CellPool.Get();
            cell.x = x;
            cell.y = y;
            cell.transform.position = GetWorldPosition(x,y);

            walkableArea[x + (y * width)] = 1;
            return cell;
        }
    }
}