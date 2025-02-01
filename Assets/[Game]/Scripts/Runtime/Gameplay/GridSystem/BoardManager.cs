using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class BoardManager : Singleton<BoardManager>
    {
        public ObjectPoolBase<GridCell> CellPool {  get; private set; }
        public GridSystem<GridCell> Board { get; private set; }

        private float cellSize = 1;
        private Vector3 originPosition;
        public int width = 10, height = 10;

        [HideInInspector]
        public int[] walkableArea;
        private void Awake()
        {
            CellPool = new(DataManager.Instance.InstanceContainer.Cell);
        }
        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
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