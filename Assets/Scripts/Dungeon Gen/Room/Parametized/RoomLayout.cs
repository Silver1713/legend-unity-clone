using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Search;

// Add this to your existing ROOM_SHAPE enum in RoomLayout.cs
public enum ROOM_SHAPE { XShape, TShape, Rectangle, LShape, RandomWalk }

public class RoomLayout
{

    public readonly int w;
    public readonly int h;


    public ICell[,] grid;


    public readonly List<Vector2Int> trapSockets = new();

    public readonly List<Vector2Int> enemySpawns = new();

    public readonly List<Vector2Int> doorCells = new();


    public readonly int seed;

    public RoomLayout(int width, int height, int seed)
    {
        w = width;
        h = height;
        this.seed = seed;
        grid = new ICell[w, h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                grid[x, y] = new ICell(new Vector2Int(x, y), CellType.Void);
            }
        }
    }


    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < w && y < h;

    public void SetCell(int x, int y, CellType c, DIRECTION direction = DIRECTION.Neutral)
    {
        if (!InBounds(x, y)) return;
        grid[x, y].cellType = c;
        grid[x, y].direction = direction;
        if (c == CellType.Door) doorCells.Add(new Vector2Int(x, y));
    }

    public ICell GetCell(int x, int y) => InBounds(x, y) ? grid[x, y] : null;

    public ICell GetCell(Vector2Int position) =>
        InBounds(position.x, position.y) ? GetCell(position.x, position.y) : null;

    // Add this method to your existing RoomLayout class
    public void LayoutRoom(ROOM_SHAPE shape)
    {
        /*switch (shape)
        {
            case ROOM_SHAPE.Rectangle:
                CarveRectangle(w, h);
                break;
            case ROOM_SHAPE.LShape:
                CarveLShape();
                break;
            case ROOM_SHAPE.TShape:
                CarveTShape();
                break;
            case ROOM_SHAPE.XShape:
                CarveCrossShape();
                break;
            case ROOM_SHAPE.RandomWalk:
                // Use the new random walk generator
                this.GenerateRandomWalk(minSize: 25, maxSize: 80, doorCount: 4, seed: this.seed);
                break;
        }*/
        this.GenerateRandomWalk(minSize: 25, maxSize: 80, doorCount: 2, cornerIntensity: 0.5f, seed: this.seed);
    }

    public void CarveLShape()
    {
        FillWalls();

        int midW = w / 2;
        int midH = h / 2;



        // Iterate through the grid
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                // From Bottom
                if (y < midH)
                {
                    // The bottom Half
                    if (y == 0)
                    {
                        DIRECTION dir = DIRECTION.Neutral;

                        if (x == 0)
                        {
                            dir = DIRECTION.BottomLeftCorner;
                        }
                        else if (x == w - 1)
                        {
                            dir = DIRECTION.BottomRightCorner;
                        }
                        else
                        {
                            dir = DIRECTION.Down;
                        }

                        SetCell(x, y, CellType.Wall, dir);

                        continue;
                    }

                    if (x == 0 || x == w - 1)
                    {

                        SetCell(x, y, CellType.Wall, (x == 0) ? DIRECTION.Left : DIRECTION.Right);
                    }
                    else if (y != 0)
                    {
                        SetCell(x, y, CellType.Floor);
                    }

                }
                else
                {
                    DIRECTION dir = DIRECTION.Neutral;

                    

                    if (y == h - 1)
                    {   
                        SetCell(x, y, CellType.Wall, DIRECTION.Up);
                    }



                    // The top Half
                    if (x == 0 || x == midW)
                    {
                        SetCell(x, y, CellType.Wall, (x == 0) ? DIRECTION.Left : DIRECTION.Right);
                    }
                    else if (x > midW)
                    {
                        if (y == midH)
                        {
                            SetCell(x, y, CellType.Wall, DIRECTION.Up);
                        }
                        else
                        {
                            SetCell(x, y, CellType.Void);
                        }
                    }
                    else if (y != h - 1)
                    {
                        SetCell(x, y, CellType.Floor);
                    }
                }
            }
        }
    }

    public void CarveTShape()
    {
        FillWalls();

        int midX = w / 2;
        int midY = h / 2;

        for (int x = 0; x < w; ++x)
        {
            for (int y = 0; y < h; ++y)
            {
                bool inHorizontalBar = (y == midY);
                bool inVerticalStem = (x == midX && y >= midY);
                if (inHorizontalBar || inVerticalStem)
                {
                    SetCell(x, y, CellType.Floor);
                    continue;
                }

                bool isBottom = (y == 0);
                bool isTop = (y == h - 1);
                bool isLeft = (x == 0);
                bool isRight = (x == w - 1);

                if (isBottom && isLeft)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.BottomLeftCorner);
                }
                else if (isBottom && isRight)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.BottomRightCorner);
                }
                else if (isTop && isLeft)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.TopLeftCorner);
                }
                else if (isTop && isRight)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.TopRightCorner);
                }
                else if (isBottom)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.Down);
                }
                else if (isTop)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.Up);
                }
                else if (isLeft)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.Left);
                }
                else if (isRight)
                {
                    SetCell(x, y, CellType.Wall, DIRECTION.Right);
                }
                else
                {
                    SetCell(x, y, CellType.Void);
                }
            }
        }
    }


    public void CarveCrossShape()
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (x == w / 2 || y == h / 2)
                {
                    SetCell(x, y, CellType.Floor);
                }
                else
                {
                    SetCell(x, y, CellType.Wall);
                }
            }
        }
    }

    public void CarveSquare()
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (x > 0 && x < w - 1 && y > 0 && y < h - 1)
                {
                    SetCell(x, y, CellType.Floor);
                    continue;
                }

                DIRECTION dir = DIRECTION.Neutral;

                if (x == 0)
                {
                    dir = (y == 0) ? DIRECTION.BottomLeftCorner :
                        (y == h - 1) ? DIRECTION.TopLeftCorner :
                        DIRECTION.Left;
                }
                else if (x == w - 1)
                {
                    dir = (y == 0) ? DIRECTION.BottomRightCorner :
                        (y == h - 1) ? DIRECTION.TopRightCorner :
                        DIRECTION.Right;
                }
                else if (y == 0)
                {
                    dir = DIRECTION.Down;
                }
                else if (y == h - 1)
                {
                    dir = DIRECTION.Up;
                }

                SetCell(x, y, CellType.Wall, dir);
            }
        }
    }

    public void CarveRectangle(int width, int height)
    {
       for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                {
                    SetCell(x, y, CellType.Floor);
                    continue;
                }
                DIRECTION dir = DIRECTION.Neutral;
                if (x == 0)
                {
                    dir = (y == 0) ? DIRECTION.BottomLeftCorner :
                        (y == height - 1) ? DIRECTION.TopLeftCorner :
                        DIRECTION.Left;
                }
                else if (x == width - 1)
                {
                    dir = (y == 0) ? DIRECTION.BottomRightCorner :
                        (y == height - 1) ? DIRECTION.TopRightCorner :
                        DIRECTION.Right;
                }
                else if (y == 0)
                {
                    dir = DIRECTION.Down;
                }
                else if (y == height - 1)
                {
                    dir = DIRECTION.Up;
                }
                SetCell(x, y, CellType.Wall, dir);
            }
        }
    }

    public void Fill(CellType c)
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                SetCell(x, y, c);
            }
        }
    }

    public void PlaceBorder(CellType c)
    {
        for (int x = 0; x < w; x++)
        {
            SetCell(x, 0, c);
            SetCell(x, h - 1, c);
        }
        for (int y = 0; y < h; y++)
        {
            SetCell(0, y, c);
            SetCell(w - 1, y, c);
        }
    }


    public void PlaceWall()
    {
        PlaceBorder(CellType.Wall);
    }


    public void PlacePlayerSpawn()
    {
        List<Vector2Int> spawnPosition = new List<Vector2Int>();
        for (int y = 2; y < h - 2; y++)
        {
            for (int x = 2; x < w - 2; x++)
            {
                if (grid[x, y].cellType == CellType.Floor)
                {
                    spawnPosition.Add(new Vector2Int(x, y));
                }
            }
        }
        if (spawnPosition.Count > 0)
        {
            Random.InitState((int)(System.DateTime.Now.Ticks));
            int randomIndex = Random.Range(0, spawnPosition.Count);
            Vector2Int spawnPos = spawnPosition[randomIndex];
            SetCell(spawnPos.x, spawnPos.y, CellType.SPAWN);
        }
        else
        {
            Debug.LogWarning("No valid spawn position found!");
        }
    }

    public void PlaceDoors(int minDoor, int maxDoor)
    {
        
        int doorCount = Random.Range(minDoor, maxDoor + 1);
        Debug.Log($"Placing {doorCount} doors in the room layout.");
        List<ICell> doorCells = GetCellsOfType(CellType.Wall);
        for (int i = 0; i < doorCount && doorCells.Count > 0; i++)
        {
            
            int randomIndex = UnityEngine.Random.Range(0, doorCells.Count);
            ICell doorCell = doorCells[randomIndex];
            DIRECTION direction = doorCell.direction;
            SetCell(doorCell.Position.x, doorCell.Position.y, CellType.Door, direction);
            doorCells.RemoveAt(randomIndex);
        }

    }

    public void FillFloor()
    {
        Fill(CellType.Floor);
    }

    public void FillWalls()
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (x == 0 || y == 0 || x == w - 1 || y == h - 1)
                {
                    SetCell(x, y, CellType.Wall);
                }
            }
        }
    }


    public List<ICell> GetCellsOfType(CellType type)
    {
        List<ICell> cells = new();
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y].cellType == type)
                {
                    cells.Add(grid[x, y]);
                }
            }
        }
        return cells;
    }

    public ICell GetSpawn()
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y].cellType == CellType.SPAWN)
                {
                    return grid[x, y];
                }
            }
        }
        return null;
    }

    public List<ICell> GetDoors()
    {
                List<ICell> doors = new();
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y].cellType == CellType.Door)
                {
                    doors.Add(grid[x, y]);
                }
            }
        }
        return doors;
    }

    public List<ICell> GetTraps()
    {
        List<ICell> traps = new();
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y].cellType == CellType.Trap)
                {
                    traps.Add(grid[x, y]);
                }
            }
        }
        return traps;
    }

    public List<ICell> GetEnemies()
    {
        List<ICell> enemies = new();
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y].cellType == CellType.Enemy)
                {
                    enemies.Add(grid[x, y]);
                }
            }
        }
        return enemies;
    }

    public List<ICell> GetTreasures()
    {
        List<ICell> treasures = new();
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y].cellType == CellType.Treasure)
                {
                    treasures.Add(grid[x, y]);
                }
            }
        }
        return treasures;
    }

    /// <summary>ASCII dump for quick console debugging.</summary>
    public string ToAscii()
    {
        System.Text.StringBuilder sb = new();
        for (int y = h - 1; y >= 0; y--)
        {
            for (int x = 0; x < w; x++)
            {
                sb.Append(
                    grid[x, y].cellType switch
                    {
                        CellType.Floor => 'F',
                        CellType.Wall => '#',
                        CellType.Door => 'D',
                        CellType.Trap => 'T',
                        CellType.Treasure => 'X',
                        CellType.Enemy => 'E',
                        CellType.Void => 'V',
                        _ => ' '
                    }
                );
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}
