
using Runtime.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridManager : Singleton<GridManager>
    {
        public GridSystem<GridCell> Board { get; private set; }
        private float cellSize = 1;
        private Vector3 originPosition;
        public int width = 10, height = 10;

        public int[] walkableArea;
        private void Awake()
        {
            Initialize();
        }
        public void Initialize()
        {
            originPosition = new Vector3(-width/2f, 0, -height/2f);
            walkableArea = new int[width * height];
            Board = new(width, height, 1,originPosition, (int x, int y) => CreateCell(x,y)); // TODO: Pool integration

        }

        public void GetGridPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        }
        public int GetIndex(int x, int y, int width, int height) => IsValidGridPosition(x, y, width, height) ? x + (y * width) : -1;

        public bool IsValidGridPosition(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
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
            walkableArea[GetIndex(x,y, width, height)] = 1;
            return cell;
        }
    }
}