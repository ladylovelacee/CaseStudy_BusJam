using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class BoardManager : Singleton<BoardManager>
    {
        [SerializeField]
        private Transform parent;
        public ObjectPoolBase<GridCell> CellPool {  get; private set; }
        public GridSystem<GridCell> Board { get; private set; }
        
        private float cellSize = 1;
        private Vector3 originPosition;
        [HideInInspector]
        public int width, height;

        [HideInInspector]
        public int[] WalkableArea {  get; private set; }
        private void Awake()
        {
            CellPool = new(DataManager.Instance.InstanceContainer.Cell);
        }

        public void Initialize(LevelData data)
        {
            width = data.width; 
            height = data.height;

            WalkableArea = new int[width * height];

            originPosition = new Vector3(-width/2f, 0, -height);
            Board = new(width, height, originPosition, (int x, int y) => CreateCell(x,y));
        }

        public Vector3 GetWorldPosition(int x, int y) => new Vector3(x, 0, y) * cellSize + originPosition;

        public void SetCellWalkable(int x, int y, bool isWalkable)
        {
            int index = Board.GetIndex(x, y);
            WalkableArea[index] = isWalkable ? 1 : 0;
        }

        private GridCell CreateCell(int x, int y)
        {
            GridCell cell = CellPool.Get();
            cell.x = x;
            cell.y = y;
            cell.transform.position = GetWorldPosition(x,y);

            WalkableArea[x + (y * width)] = 1;

            cell.transform.SetParent(parent);
            return cell;
        }
    }
}