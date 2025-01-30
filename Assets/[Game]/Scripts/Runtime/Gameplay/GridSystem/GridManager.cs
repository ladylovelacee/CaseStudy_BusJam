using DG.Tweening;
using Runtime.Core;
using System.Collections.Generic;
using Unity.Collections;
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

        private GridCell CreateCell(int x, int y) {
            GridCell cell = Instantiate(DataManager.Instance.InstanceContainer.Cell, GetWorldPosition(x,y), Quaternion.identity);
            cell.x = x;
            cell.y = y;

            return cell;
        }

        //public void FindPath(GridCell startNode, GridCell targetNode)
        //{
        //    //GridCell[] pathNodeArray = new GridCell[width * height];
        //    //NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        //    //NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        //    //for (int i = 0; i < width; i++)
        //    //{
        //    //    for (int j = 0; j < height; j++)
        //    //    {
        //    //        GridCell cell = Board.GetGridObject(i, j);
        //    //        if (!cell.isWalkable) continue;
                    
        //    //        cell.cameFromNodeIndex = -1;
        //    //        cell.gCost = int.MaxValue;
        //    //        cell.hCost =  CalculateDistanceCost()
        //    //    }
        //    //}
        //}

        //private int CalculateDistanceCost(Vector2Int a, Vector2Int b)=> Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
       
        //private NativeList<Vector2Int> CalculatePath(GridCell[] pathNodeArray, GridCell endNode)
        //{
        //    if (endNode.cameFromNodeIndex == -1)
        //    {
        //        // Couldn't find a path!
        //        return new NativeList<Vector2Int>(Allocator.Temp);
        //    }
        //    else
        //    {
        //        // Found a path
        //        NativeList<Vector2Int> path = new NativeList<Vector2Int>(Allocator.Temp);
        //        path.Add(new Vector2Int(endNode.x, endNode.y));

        //        GridCell currentNode = endNode;
        //        while (currentNode.cameFromNodeIndex != -1)
        //        {
        //            GridCell cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
        //            path.Add(new Vector2Int(cameFromNode.x, cameFromNode.y));
        //            currentNode = cameFromNode;
        //        }

        //        return path;
        //    }
        //} 
    }
}