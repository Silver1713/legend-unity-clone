using System.Collections.Generic;
using UnityEngine;

public class RandomWalkRoomGenerator
{
    private RoomLayout roomLayout;
    private int width;
    private int height;
    private int minSize;
    private int maxSize;
    private float cornerIntensity;
    private int doorCount;

    // Directions for moving around (orthogonal and diagonal)
    private readonly Vector2Int[] omnidirections = { 
        new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1),
        new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1) 
    };

    public RandomWalkRoomGenerator(int width, int height, int doorCount, float cornerIntensity, int minSize, int maxSize)
    {
        this.width = width;
        this.height = height;
        this.doorCount = doorCount;
        this.minSize = minSize;
        this.maxSize = maxSize;
        this.cornerIntensity = cornerIntensity;
    }

    public RoomLayout GenerateRoom(int seed)
    {
        Random.InitState(seed);
        roomLayout = new RoomLayout(width, height, seed);

        // Step 1: Initialize the dungeon with empty space
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                roomLayout.SetCell(x, y, CellType.Void);
            }
        }

        // Step 2: Randomly choose a starting point for the room
        int startX = Random.Range(1, width - 1);
        int startY = Random.Range(1, height - 1);
        roomLayout.SetCell(startX, startY, CellType.Floor);

        // Step 3: Procedurally expand the room using random walk
        ExpandRoom(startX, startY, 3 * (width + height));

        // Step 4: Enclose the room with walls
        EncloseRoomWithWalls();

        // Step 5: Place doors
        PlaceRoomDoors();

        return roomLayout;
    }

    private void ExpandRoom(int startX, int startY, int maxSteps)
    {
        int currentX = startX;
        int currentY = startY;
        roomLayout.SetCell(currentX, currentY, CellType.Floor);
        int size = 1;

        // Directions for random walk (orthogonal only)
        var directions = new List<Vector2Int>
        {
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(0, 1),  // Up
        };
        
        int prev_dir = -1;
        // Walk for a maximum number of steps or until we hit size limit
        for (int step = 0; (step < maxSteps && size < maxSize); step++)
        {
            // Randomly choose a direction
            int dir = Random.Range(0, directions.Count);
            dir = (Random.Range(0f, 1f) < cornerIntensity || prev_dir==-1) ? dir : prev_dir;
            Vector2Int direction = directions[dir];
            
            // Calculate the new position
            int nx = currentX + direction.x;
            int ny = currentY + direction.y;

            // Check if the new position is valid
            if (IsValidExpansion(nx, ny))
            {
                // Mark the new tile as a floor tile
                roomLayout.SetCell(nx, ny, CellType.Floor);
                size++;

                // Update current position
                currentX = nx;
                currentY = ny;
            }
            // If the move is invalid, continue with same position
            prev_dir = dir;
        }

        // Ensure minimum size is met
        if (size < minSize)
        {
            ExpandRoomToMinimumSize(size);
        }
    }

    private bool IsValidExpansion(int x, int y)
    {
        // Check bounds with buffer for walls
        if (x <= 0 || y <=0 || x >= width - 1 || y >= height - 1)
            return false;

        // Check if already a floor tile
        return roomLayout.GetCell(x, y)?.cellType != CellType.Floor;
    }

    private void ExpandRoomToMinimumSize(int currentSize)
    {
        var floorCells = roomLayout.GetCellsOfType(CellType.Floor);
        
        while (currentSize < minSize && floorCells.Count > 0)
        {
            // Pick a random floor cell and try to expand from it
            var randomFloor = floorCells[Random.Range(0, floorCells.Count)];
            var pos = randomFloor.Position;

            // Try all directions from this cell
            var directions = new Vector2Int[] {
		new Vector2Int(-1, 0), new Vector2Int(1, 0), 
                new Vector2Int(0, -1), new Vector2Int(0, 1)
            };

            bool expanded = false;
            foreach (var dir in directions)
            {
                int nx = pos.x + dir.x;
                int ny = pos.y + dir.y;

                if (IsValidExpansion(nx, ny))
                {
                    roomLayout.SetCell(nx, ny, CellType.Floor);
                    currentSize++;
                    expanded = true;
                    break;
                }
            }

            if (!expanded)
            {
                floorCells.Remove(randomFloor);
            }
        }
    }

    private void EncloseRoomWithWalls()
    {
        // Create a copy to avoid modifying while iterating
        CellType[,] originalGrid = new CellType[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                originalGrid[x, y] = roomLayout.GetCell(x, y).cellType;
            }
        }

        // Check each empty cell to see if it should become a wall
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (originalGrid[x, y] == CellType.Void)
                {
                    // Check if this void cell is adjacent to a floor
                    bool adjacentToFloor = false;
                    foreach (var dir in omnidirections)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;

                        if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                        {
                            if (originalGrid[nx, ny] == CellType.Floor)
                            {
                                adjacentToFloor = true;
                                break;
                            }
                        }
                    }

                    if (adjacentToFloor)
                    {
                        // Determine wall direction
                        DIRECTION wallDirection = DetermineWallDirection(x, y, originalGrid);
                        roomLayout.SetCell(x, y, CellType.Wall, wallDirection);
                    }
                }
                // Set border walls
                else if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    if (originalGrid[x, y] != CellType.Floor)
                    {
                        DIRECTION wallDirection = DetermineWallDirection(x, y, originalGrid);
                        roomLayout.SetCell(x, y, CellType.Wall, wallDirection);
                    }
                }
            }
        }
    }

private DIRECTION DetermineWallDirection(int x, int y, CellType[,] grid)
{
    // Check all adjacent tiles for floors
    bool hasFloorAbove = (y < height - 1) && grid[x, y + 1] == CellType.Floor;
    bool hasFloorBelow = (y > 0) && grid[x, y - 1] == CellType.Floor;
    bool hasFloorLeft = (x > 0) && grid[x - 1, y] == CellType.Floor;
    bool hasFloorRight = (x < width - 1) && grid[x + 1, y] == CellType.Floor;
    
    // Check diagonal tiles for floors (needed for corner detection)
    bool hasFloorTopLeft = (x > 0 && y < height - 1) && grid[x - 1, y + 1] == CellType.Floor;
    bool hasFloorTopRight = (x < width - 1 && y < height - 1) && grid[x + 1, y + 1] == CellType.Floor;
    bool hasFloorBottomLeft = (x > 0 && y > 0) && grid[x - 1, y - 1] == CellType.Floor;
    bool hasFloorBottomRight = (x < width - 1 && y > 0) && grid[x + 1, y - 1] == CellType.Floor;
    // First, check for corners (internal and external)
    // A corner is where floors are adjacent in two perpendicular directions or diagonally
    
    // Bottom-left corner: floor to the right and/or above, or diagonally top-right
    if ((hasFloorRight && hasFloorAbove) || (!hasFloorRight && !hasFloorAbove && hasFloorTopRight))
    {
        return DIRECTION.BottomLeftCorner;
    }
    
    // Bottom-right corner: floor to the left and/or above, or diagonally top-left
    if ((hasFloorLeft && hasFloorAbove) || (!hasFloorLeft && !hasFloorAbove && hasFloorTopLeft))
    {
        return DIRECTION.BottomRightCorner;
    }
    
    // Top-left corner: floor to the right and/or below, or diagonally bottom-right
    if ((hasFloorRight && hasFloorBelow) || (!hasFloorRight && !hasFloorBelow && hasFloorBottomRight))
    {
        return DIRECTION.TopLeftCorner;
    }
    
    // Top-right corner: floor to the left and/or below, or diagonally bottom-left
    if ((hasFloorLeft && hasFloorBelow) || (!hasFloorLeft && !hasFloorBelow && hasFloorBottomLeft))
    {
        return DIRECTION.TopRightCorner;
    }

    // Now check for straight walls (only one side has floor)
    if (hasFloorAbove && !hasFloorBelow && !hasFloorLeft && !hasFloorRight)
        return DIRECTION.Down;
    if (hasFloorBelow && !hasFloorAbove && !hasFloorLeft && !hasFloorRight)
        return DIRECTION.Up;
    if (hasFloorLeft && !hasFloorRight && !hasFloorAbove && !hasFloorBelow)
        return DIRECTION.Right;
    if (hasFloorRight && !hasFloorLeft && !hasFloorAbove && !hasFloorBelow)
        return DIRECTION.Left;

    // Handle edge cases where wall might be on multiple sides
    // Prioritize based on the stronger connection (more adjacent floors)
    int verticalFloors = (hasFloorAbove ? 1 : 0) + (hasFloorBelow ? 1 : 0);
    int horizontalFloors = (hasFloorLeft ? 1 : 0) + (hasFloorRight ? 1 : 0);
    
    if (verticalFloors > horizontalFloors)
    {
        if (hasFloorAbove) return DIRECTION.Down;
        if (hasFloorBelow) return DIRECTION.Up;
    }
    else if (horizontalFloors > verticalFloors)
    {
        if (hasFloorLeft) return DIRECTION.Right;
        if (hasFloorRight) return DIRECTION.Left;
    }
    
    // If we still haven't determined direction, use position-based fallback
    // This handles outer walls of the map
    if (y == height - 1) return DIRECTION.Up;
    if (y == 0) return DIRECTION.Down;
    if (x == 0) return DIRECTION.Left;
    if (x == width - 1) return DIRECTION.Right;
    
    // Last resort - pick based on any adjacent floor
    if (hasFloorAbove) return DIRECTION.Down;
    if (hasFloorBelow) return DIRECTION.Up;
    if (hasFloorLeft) return DIRECTION.Right;
    if (hasFloorRight) return DIRECTION.Left;
    
    // This should rarely happen, but if it does, default to Down
    Debug.LogWarning($"Wall at ({x}, {y}) could not determine direction, defaulting to Down");
    return DIRECTION.Down;
}
/*
    private DIRECTION DetermineWallDirection(int x, int y, CellType[,] grid)
{
    bool hasFloorAbove = (y < height - 1) && grid[x, y + 1] == CellType.Floor;
    bool hasFloorBelow = (y > 0) && grid[x, y - 1] == CellType.Floor;
    bool hasFloorLeft  = (x > 0) && grid[x - 1, y] == CellType.Floor;
    bool hasFloorRight = (x < width - 1) && grid[x + 1, y] == CellType.Floor;

    // Single-wall segments
    if (hasFloorAbove && !hasFloorBelow && !hasFloorLeft  && !hasFloorRight) return DIRECTION.Down;
    if (hasFloorBelow && !hasFloorAbove && !hasFloorLeft  && !hasFloorRight) return DIRECTION.Up;
    if (hasFloorLeft  && !hasFloorRight && !hasFloorAbove && !hasFloorBelow) return DIRECTION.Right;
    if (hasFloorRight && !hasFloorLeft  && !hasFloorAbove && !hasFloorBelow) return DIRECTION.Left;

    // Interior corner segments (new!)
    // Floor above + floor to right  => bottom-left corner
    if (hasFloorAbove && hasFloorRight  && !hasFloorBelow && !hasFloorLeft)
        return DIRECTION.BottomLeftCorner;
    // Floor above + floor to left   => bottom-right corner
    if (hasFloorAbove && hasFloorLeft   && !hasFloorBelow && !hasFloorRight)
        return DIRECTION.BottomRightCorner;
    // Floor below + floor to right  => top-left corner
    if (hasFloorBelow && hasFloorRight  && !hasFloorAbove && !hasFloorLeft)
        return DIRECTION.TopLeftCorner;
    // Floor below + floor to left   => top-right corner
    if (hasFloorBelow && hasFloorLeft   && !hasFloorAbove && !hasFloorRight)
        return DIRECTION.TopRightCorner;

    // Map‐edge corners
    if (x == 0 && y == 0) return DIRECTION.BottomLeftCorner;
    if (x == width - 1 && y == 0) return DIRECTION.BottomRightCorner;
    if (x == 0 && y == height - 1) return DIRECTION.TopLeftCorner;
    if (x == width - 1 && y == height - 1) return DIRECTION.TopRightCorner;

    // Map edges (just straight walls)
    if (x == 0)              return DIRECTION.Left;
    if (x == width - 1)      return DIRECTION.Right;
    if (y == 0)              return DIRECTION.Down;
    if (y == height - 1)     return DIRECTION.Up;

    // Anything else was ambiguous
    return DIRECTION.Neutral;
}*/


    private void PlaceRoomDoors()
    {
        int actualDoorCount = Random.Range(1, doorCount + 1);
        var directions = new List<Vector2Int>
        {
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(0, 1),  // Up
        };

        List<Vector2Int> doors = new List<Vector2Int>();
        var wallCells = roomLayout.GetCellsOfType(CellType.Wall);

        for (int i = 0; (i < actualDoorCount || doors.Count == 0) && doors.Count < actualDoorCount; i++)
        {
            foreach (var wallCell in wallCells)
            {
                if (doors.Count >= actualDoorCount) break;

                Vector2Int pos = wallCell.Position;
                if (IsValidDoorPosition(pos, directions, doors))
                {
                    if (Random.Range(0, 100) < 25) // 25% chance to place door
                    {
                        DIRECTION doorDirection = wallCell.direction;
                        bool isvaliddoubledoor = false;
                        if (doorDirection == DIRECTION.Down || doorDirection == DIRECTION.Up){
                            Vector2Int d2pos = pos;
                            d2pos.x+=1;
                            isvaliddoubledoor = IsValidDoorPosition(d2pos, directions, doors);
                        }
                        else{
                            Vector2Int d2pos = pos;
                            d2pos.y+=1;
                            isvaliddoubledoor = IsValidDoorPosition(d2pos, directions, doors);
                        }
                        if(isvaliddoubledoor){
                            roomLayout.SetCell(pos.x, pos.y, CellType.Door, doorDirection);
                            doors.Add(pos);
                        }
                    }
                }
            }
        }

        Debug.Log($"Placed {doors.Count} doors in the room.");
    }

    private bool IsValidDoorPosition(Vector2Int pos, List<Vector2Int> directions, List<Vector2Int> existingDoors)
    {
        // Check if this position would create a door corridor
        int wallNeighbors = 0;
        Vector2Int wallSum = Vector2Int.zero;

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = pos + dir;
            
            if (neighborPos.x >= 0 && neighborPos.y >= 0 && 
                neighborPos.x < width && neighborPos.y < height)
            {
                var neighborCell = roomLayout.GetCell(neighborPos.x, neighborPos.y);
                if (neighborCell?.cellType == CellType.Wall)
                {
                    wallSum += dir;
                    wallNeighbors++;
                }
            }
        }

        // Valid door position: exactly 2 wall neighbors that are opposite each other
        if (wallNeighbors == 2 && wallSum == Vector2Int.zero)
        {
            // Check minimum distance from existing doors
            foreach (var existingDoor in existingDoors)
            {
                if (Vector2Int.Distance(pos, existingDoor) < 3)
                {
                    return false;
                }
            }
            return true;
        }

        ;
        return false;
    }
}

// Extension class for the existing RoomLayout to support random walk generation
public static class RoomLayoutExtensions
{
    public static void GenerateRandomWalk(this RoomLayout layout, int minSize, int maxSize, float cornerIntensity, int doorCount, int seed)
    {
        var generator = new RandomWalkRoomGenerator(layout.w, layout.h, doorCount, cornerIntensity, minSize, maxSize);
        var generatedLayout = generator.GenerateRoom(seed);
        
        // Copy the generated layout to this layout
        for (int x = 0; x < layout.w; x++)
        {
            for (int y = 0; y < layout.h; y++)
            {
                var cell = generatedLayout.GetCell(x, y);
                if (cell != null)
                {
                    layout.SetCell(x, y, cell.cellType, cell.direction);
                }
            }
        }
    }
}
