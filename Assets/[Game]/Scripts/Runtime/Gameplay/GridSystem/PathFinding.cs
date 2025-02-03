using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using System;
using UnityEngine;
using DG.Tweening;

namespace Runtime.Gameplay
{
    public class Pathfinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 24;

        private static readonly int2[] neighbourOffsets = new int2[]
        {
            new int2(-1, 0),  // Left
            new int2(+1, 0),  // Right
            new int2(0, +1),  // Up
            new int2(0, -1)   // Down
        };

        public static JobHandle FindPath(int2 gridSize, int2 start, int2 end, NativeArray<int> walkableArea, out NativeList<int2> path)
        {
            path = new NativeList<int2>(Allocator.Persistent);

            FindPathJob job = new FindPathJob
            {
                startPosition = start,
                endPosition = end,
                gridSize = gridSize,
                walkableArea = walkableArea,
                findedPath = path
            };

            return job.Schedule();
        }

        [BurstCompile]
        private struct FindPathJob : IJob
        {
            public int2 startPosition;
            public int2 endPosition;
            public int2 gridSize;
            [ReadOnly] public NativeArray<int> walkableArea;
            public NativeList<int2> findedPath;

            public void Execute()
            {
                int totalNodes = gridSize.x * gridSize.y;
                NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(totalNodes, Allocator.Temp);

                // PathNode dizisini baþlat
                for (int i = 0; i < totalNodes; i++)
                {
                    int x = i % gridSize.x;
                    int y = i / gridSize.x;

                    pathNodeArray[i] = new PathNode
                    {
                        x = x,
                        y = y,
                        index = i,
                        gCost = int.MaxValue,
                        hCost = CalculateDistanceCost(new int2(x, y), endPosition),
                        isWalkable = walkableArea[i] == 1,
                        cameFromNodeIndex = -1
                    };
                    pathNodeArray[i].CalculateFCost();
                }

                int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);
                int startNodeIndex = CalculateIndex(startPosition.x, startPosition.y, gridSize.x);

                PathNode startNode = pathNodeArray[startNodeIndex];
                startNode.gCost = 0;
                startNode.CalculateFCost();
                pathNodeArray[startNodeIndex] = startNode;

                NativeMinHeap openList = new NativeMinHeap(totalNodes, Allocator.Temp);
                NativeHashSet<int> closedList = new NativeHashSet<int>(totalNodes, Allocator.Temp);

                openList.Push(startNode.index, startNode.fCost);

                while (!openList.IsEmpty)
                {
                    int currentNodeIndex = openList.Pop();
                    PathNode currentNode = pathNodeArray[currentNodeIndex];

                    if (currentNodeIndex == endNodeIndex)
                    {
                        // Hedefe ulaþýldý!
                        break;
                    }

                    closedList.Add(currentNodeIndex);

                    foreach (var neighbourOffset in neighbourOffsets)
                    {
                        int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);
                        if (!IsPositionInsideGrid(neighbourPosition, gridSize)) continue;

                        int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                        if (closedList.Contains(neighbourNodeIndex)) continue;

                        PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                        if (!neighbourNode.isWalkable) continue;

                        int tentativeGCost = currentNode.gCost + MOVE_STRAIGHT_COST;
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.cameFromNodeIndex = currentNodeIndex;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.CalculateFCost();
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;

                            openList.Push(neighbourNode.index, neighbourNode.fCost);
                        }
                    }
                }

                PathNode endNode = pathNodeArray[endNodeIndex];
                if (endNode.cameFromNodeIndex != -1)
                    CalculatePath(pathNodeArray, endNode);

                pathNodeArray.Dispose();
                openList.Dispose();
                closedList.Dispose();
            }

            private void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
            {
                findedPath.Add(new int2(endNode.x, endNode.y));
                PathNode currentNode = endNode;

                while (currentNode.cameFromNodeIndex != -1)
                {
                    currentNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    findedPath.Add(new int2(currentNode.x, currentNode.y));
                }
            }

            private bool IsPositionInsideGrid(int2 pos, int2 size) => pos.x >= 0 && pos.y >= 0 && pos.x < size.x && pos.y < size.y;

            private int CalculateIndex(int x, int y, int width) => x + y * width;

            private int CalculateDistanceCost(int2 a, int2 b)
            {
                int xDist = math.abs(a.x - b.x);
                int yDist = math.abs(a.y - b.y);
                return MOVE_STRAIGHT_COST * (xDist + yDist);
            }
        }
    }
    public struct NativeMinHeap : IDisposable
    {
        private NativeList<int> heap;
        private NativeArray<int> costs;

        public NativeMinHeap(int capacity, Allocator allocator)
        {
            heap = new NativeList<int>(capacity, allocator);
            costs = new NativeArray<int>(capacity, allocator);
        }

        public void Push(int index, int cost)
        {
            heap.Add(index);
            costs[index] = cost;
            HeapifyUp(heap.Length - 1);
        }

        public int Pop()
        {
            if (heap.Length == 0) return -1;

            int minIndex = heap[0];
            heap[0] = heap[heap.Length - 1];
            heap.RemoveAt(heap.Length - 1);
            HeapifyDown(0);

            return minIndex;
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (costs[heap[i]] >= costs[heap[parent]]) break;

                (heap[i], heap[parent]) = (heap[parent], heap[i]);
                i = parent;
            }
        }

        private void HeapifyDown(int i)
        {
            int lastIndex = heap.Length - 1;
            while (true)
            {
                int leftChild = 2 * i + 1;
                int rightChild = 2 * i + 2;
                int smallest = i;

                if (leftChild <= lastIndex && costs[heap[leftChild]] < costs[heap[smallest]]) smallest = leftChild;
                if (rightChild <= lastIndex && costs[heap[rightChild]] < costs[heap[smallest]]) smallest = rightChild;

                if (smallest == i) break;
                (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
                i = smallest;
            }
        }

        public bool IsEmpty => heap.Length == 0;

        public void Dispose()
        {
            heap.Dispose();
            costs.Dispose();
        }
    }

    public struct PathNode
    {
        public int x;
        public int y;

        public int index;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;

        public int cameFromNodeIndex;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }
    }
}
