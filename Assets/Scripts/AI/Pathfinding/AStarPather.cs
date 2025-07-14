using System.Collections.Generic;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class AStarPather
{
    public PathNode[,] grid;



    public AStarPather(int width, int height)
    {
        grid = new PathNode[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new PathNode(new Vector2Int(x, y), 0, 0);
            }
        }
    }

    public AStarPather(RoomLayout layout)
    {
        grid = new PathNode[layout.w, layout.h];
        for (int x = 0; x < layout.w; x++)
        {
            for (int y = 0; y < layout.h; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                float gCost = 0; // Initialize gCost as needed
                float hCost = 0; // Initialize hCost as needed
                ICell cell = layout.GetCell(position);
                if (cell.cellType == CellType.Wall || cell.cellType == CellType.Trap || cell.CellType == CellType.Void)
                {
                    grid[x, y] = new PathNode(position, gCost, hCost, true); // Mark as non-path if it's a wall 

                }
                else
                    grid[x, y] = new PathNode(position, gCost, hCost);

            }
        }
    }

    public PathNode GetNode(Vector2Int position)
    {
        if (position.x < 0 || position.x >= grid.GetLength(0) || position.y < 0 || position.y >= grid.GetLength(1))
        {
            throw new System.IndexOutOfRangeException("Position is out of bounds of the grid.");
        }

        return grid[position.x, position.y];
    }

    public void SetNode(Vector2Int position, PathNode node)
    {
        if (position.x < 0 || position.x >= grid.GetLength(0) || position.y < 0 || position.y >= grid.GetLength(1))
        {
            throw new System.IndexOutOfRangeException("Position is out of bounds of the grid.");
        }

        grid[position.x, position.y] = node;
    }

    public void ClearGrid()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new PathNode(new Vector2Int(x, y), 0, 0);
            }
        }
    }


    public void PrintAscii()
    {
        var sb = new StringBuilder("A* Pathfinding Grid:\n");
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int y = height - 1; y >= 0; y--) // top to bottom
        {
            for (int x = 0; x < width; x++)
            {
                PathNode node = grid[x, y];
                sb.Append(node.IsNonPath() ? 'X' : 'O');
            }

            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }

    public List<PathNode> GetPath(Vector2Int start, Vector2Int end)
    {

        ;

        PriorityQueue<PathNode, float> openSet = new PriorityQueue<PathNode, float>();

        PathNode startNode = GetNode(start);
        PathNode endNode = GetNode(end);

        openSet.Enqueue(startNode, startNode.fCost);

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet.Dequeue();
            if (currentNode.position == endNode.position)
            {
                return ReconstructPath(currentNode);
            }
            foreach (PathNode neighbor in GetNeighboors(currentNode.position))
            {
                if (neighbor.IsNonPath() || neighbor.nodeType == PathNode.NodeType.Closed)
                    continue;
                float tentativeGCost = currentNode.gCost + Vector2Int.Distance(currentNode.position, neighbor.position);
                if (tentativeGCost < neighbor.gCost || neighbor.gCost == 0)
                {
                    neighbor.parent = currentNode;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = Vector2Int.Distance(neighbor.position, endNode.position);
                    openSet.Enqueue(neighbor, neighbor.fCost);
                    SetNode(neighbor.position, neighbor);
                }
            }
            currentNode.nodeType = PathNode.NodeType.Closed; // Mark the current node as closed
        }

        Debug.LogWarning("No path found from " + start + " to " + end);
        return new List<PathNode>(); // Return an empty path if no path is found

    }


    public List<PathNode> ReconstructPath(PathNode node)
    {
        PathNode currentNode = node;
        List<PathNode> path = new List<PathNode>();
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // Reverse the path to get it from start to end
        return path;
    }

    public PathNode[] GetNeighboors(Vector2Int pos)
    {
        PathNode[] neighbors = new PathNode[4];
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), // Right
            new Vector2Int(-1, 0), // Left
            new Vector2Int(0, 1), // Up
            new Vector2Int(0, -1) // Down
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int neighborPos = pos + directions[i];
            if (neighborPos.x >= 0 && neighborPos.x < grid.GetLength(0) && neighborPos.y >= 0 &&
                neighborPos.y < grid.GetLength(1))
            {
                neighbors[i] = GetNode(neighborPos);
            }
            else
            {
                neighbors[i] = new PathNode(neighborPos, 0, 0, true); // Non-path node if out of bounds
            }
        }

        return neighbors;
    }
}



    