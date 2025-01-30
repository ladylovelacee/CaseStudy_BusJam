using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridSystem<T>
    {
        private T[,] grid;
        private int width, height;
        private float cellSize;
        private Vector3 originPosition;

        public readonly Vector2Int[] Directions = new Vector2Int[4]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

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

        public void SetGridObject(int x, int y, T value)
        {
            if (IsValidGridPosition(x, y))
            {
                grid[x, y] = value;
            }
        }

        public T GetGridObject(int x, int y)
        {
            return IsValidGridPosition(x, y) ? grid[x, y] : default;
        }

        public int GetIndex(int x, int y) => IsValidGridPosition(x, y) ? x + (y*width) : -1;
        public void GetXYFromIndex(int index, out int x, out int y)
        {
            x = index % width;
            y = index / width;
        }

        public bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        private List<Vector2Int> GetNeighbors(Vector2Int position)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            foreach (var dir in Directions)
            {
                Vector2Int neighbor = position + dir;
                if (IsValidGridPosition(neighbor.x, neighbor.y))
                {
                    neighbors.Add(neighbor);
                }
            }
            return neighbors;
        }
    }    
}