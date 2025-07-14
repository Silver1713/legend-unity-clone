using System;
using UnityEngine;

public class PathNode : IComparable<PathNode>
{
    public enum NodeType
    {
        Open,
        Closed
    }

    public NodeType nodeType = NodeType.Open;
    public Vector2Int position;
    public float gCost;
    public float hCost;


    private bool nonPath;
    public float fCost => gCost + hCost;

    public PathNode parent = null;

    public PathNode(Vector2Int position, float gCost, float hCost, bool nonPath = false)
    {
        this.position = position;
        this.gCost = gCost;
        this.hCost = hCost;
        this.nonPath = nonPath;
    }


    public bool IsNonPath()
    {
        return nonPath;
    }

    public int CompareTo(PathNode other)
    {
        if (fCost < other.fCost) return -1;
        if (fCost > other.fCost) return 1;
        if (hCost < other.hCost) return -1;
        if (hCost > other.hCost) return 1;
        return 0;
    }
}
