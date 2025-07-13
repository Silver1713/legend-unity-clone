using UnityEngine;

public enum CellType { Void, Floor, Wall, Door, Trap, Treasure, Enemy, SPAWN}
public enum DIRECTION { TopLeftCorner,TopRightCorner, BottomLeftCorner, BottomRightCorner,Up, Down, Left, Right, Neutral }

public class ICell
{
    public CellType cellType;
    public DIRECTION direction;
    Vector2Int position;
    public bool isWalkable; 
    public Vector2Int Position
    {
        get => position;
        set => position = value;
    }
    public ICell(Vector2Int position, CellType cellType)
    {
        this.position = position;
        this.cellType = cellType;
    }

    public CellType CellType
    {
        get => cellType;
        set => cellType = value;
    }

    public override string ToString()
    {
        return $"Cell Type: {cellType}, Position: {position}";
    }



}
