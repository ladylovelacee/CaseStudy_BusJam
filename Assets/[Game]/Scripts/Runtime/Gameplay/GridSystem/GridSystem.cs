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

        public GridSystem(int width, int height, float cellSize, Vector3 originPosition, Func<int, int, T> createDefaultItem)
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
    }
}
