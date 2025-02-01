using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridSystem<T>
    {
        public T[,] grid;
        private int width, height;
        private float cellSize;
        private Vector3 originPosition;

        public GridSystem(int width, int height, Vector3 originPosition, Func<int, int, T> createDefaultItem, float cellSize = 1)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            grid = new T[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = createDefaultItem(x, y);
                }
            }
        }

        public void GetXYFromIndex(int index, out int x, out int y)
        {
            x = index % width;
            y = index / width;
        }

        public bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public int GetIndex(int x, int y) => IsValidGridPosition(x, y) ? x + (y * width) : -1;

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * cellSize + originPosition;
        }

        public void GetGridPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        }
    }
}
