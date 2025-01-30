using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace Runtime.Gameplay
{
    public class Pathfinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public static JobHandle FindPath(int[,] walkable, int2 gridSize, int2 start, int2 end, out NativeList<int2> path)
        {
            path = new NativeList<int2>(Allocator.TempJob);

            NativeArray<PathNode> nodes = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.TempJob);
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), end);
                    pathNode.CalculateFCost();
                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.isWalkable = walkable[x,y] == 1 ? true : false;
                    pathNode.cameFromNodeIndex = -1;
                    nodes[pathNode.index] = pathNode;
                }
            }

            FindPathJob job = new FindPathJob
            {
                startPosition = start,
                endPosition = end,
                gridSize = gridSize,
                path = new NativeList<int2>(Allocator.TempJob),
                pathNodeArray = nodes
            };

            return job.Schedule();
        }
        public static int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }

        public static int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        [BurstCompile]
        private struct FindPathJob : IJob
        {
            public int2 startPosition;
            public int2 endPosition;
            public int2 gridSize;
            public NativeArray<PathNode> pathNodeArray;
            public NativeList<int2> path;

            public void Execute()
            {
                NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(4, Allocator.Temp);
                neighbourOffsetArray[0] = new int2(-1, 0); // Left
                neighbourOffsetArray[1] = new int2(+1, 0); // Right
                neighbourOffsetArray[2] = new int2(0, +1); // Up
                neighbourOffsetArray[3] = new int2(0, -1); // Down


                int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

                PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
                startNode.gCost = 0;
                startNode.CalculateFCost();
                pathNodeArray[startNode.index] = startNode;

                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

                openList.Add(startNode.index);

                while (openList.Length > 0)
                {
                    int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                    PathNode currentNode = pathNodeArray[currentNodeIndex];

                    if (currentNodeIndex == endNodeIndex)
                    {
                        // Reached our destination!
                        break;
                    }

                    // Remove current node from Open List
                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currentNodeIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closedList.Add(currentNodeIndex);

                    for (int i = 0; i < neighbourOffsetArray.Length; i++)
                    {
                        int2 neighbourOffset = neighbourOffsetArray[i];
                        int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                        {
                            // Neighbour not valid position
                            continue;
                        }

                        int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                        if (closedList.Contains(neighbourNodeIndex))
                        {
                            // Already searched this node
                            continue;
                        }

                        PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                        if (!neighbourNode.isWalkable)
                        {
                            // Not walkable
                            continue;
                        }

                        int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.cameFromNodeIndex = currentNodeIndex;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.CalculateFCost();
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNode.index))
                            {
                                openList.Add(neighbourNode.index);
                            }
                        }

                    }
                }

                PathNode endNode = pathNodeArray[endNodeIndex];
                if (endNode.cameFromNodeIndex == -1)
                {
                    // Didn't find a path!
                    //Debug.Log("Didn't find a path!");
                }
                else
                {
                    // Found a path
                    path = CalculatePath(pathNodeArray, endNode);
                }

                path.Dispose();
                pathNodeArray.Dispose();
                neighbourOffsetArray.Dispose();
                openList.Dispose();
                closedList.Dispose();
            }

            private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
            {
                if (endNode.cameFromNodeIndex == -1)
                {
                    // Couldn't find a path!
                    return new NativeList<int2>(Allocator.Temp);
                }
                else
                {
                    // Found a path
                    NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                    path.Add(new int2(endNode.x, endNode.y));

                    PathNode currentNode = endNode;
                    while (currentNode.cameFromNodeIndex != -1)
                    {
                        PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                        path.Add(new int2(cameFromNode.x, cameFromNode.y));
                        currentNode = cameFromNode;
                    }

                    return path;
                }
            }

            private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
            {
                return
                    gridPosition.x >= 0 &&
                    gridPosition.y >= 0 &&
                    gridPosition.x < gridSize.x &&
                    gridPosition.y < gridSize.y;
            }


            private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
            {
                PathNode lowestCostPathNode = pathNodeArray[openList[0]];
                for (int i = 1; i < openList.Length; i++)
                {
                    PathNode testPathNode = pathNodeArray[openList[i]];
                    if (testPathNode.fCost < lowestCostPathNode.fCost)
                    {
                        lowestCostPathNode = testPathNode;
                    }
                }
                return lowestCostPathNode.index;
            }


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