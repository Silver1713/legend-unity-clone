using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int numEnemy;

        public int minimum;
        public int maximum;


        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Arrays of tile prefabs
    public GameObject[] floorTiles;
    public GameObject[] topWallsTiles;
    public GameObject[] bottomWallsTiles;
    public GameObject[] leftWallsTiles;
    public GameObject[] rightWallsTiles;
    public GameObject topLeftCornerTile;
    public GameObject topRightCornerTile;
    public GameObject bottomLeftCornerTile;
    public GameObject bottomRightCornerTile;
    public GameObject innertopLeftCornerTile;
    public GameObject innertopRightCornerTile;
    public GameObject innerbottomLeftCornerTile;
    public GameObject innerbottomRightCornerTile;

    public List<Doorway> DoorwayPrefabs;
    public GameObject switchPrefab;
    public GameObject[] enemyPrefabs;

    public PCGParams Params;


    public int enemies = 0;

    enum ENEMY_TYPE
    {
        RANGE,
        SWARM,
        CONTACT
    }


    
    // A list of possible locations to place tiles.
    //private List<Vector3> gridPositions = new List<Vector3>();

    private int _adjacentOffsetX;
    private int _adjacentOffsetY;

    private Room room;

    private GameObject switchInstance;

    private List<Position> _adjacentRooms;


    public RoomLayout currentRoom;
    private bool onSpawn = true;
    public Room GenerateRoom(int offsetX, int offsetY, List<Position> doorways, int roomId)
    {
        _adjacentOffsetX = offsetX;
        _adjacentOffsetY = offsetY;

        _adjacentRooms = doorways;
        Random.InitState(roomId); // Initialize the random state with the room ID

        room = new Room(roomId);
        RoomLayout layout = RoomGenerator.GenerateRoomLayout(Const.MapWitdth, Const.MapHeight, new GeneratorInfo
        {
            Seed = roomId,
            RoomCount = _adjacentRooms.Count,
            doors = new List<DOOR_TYPE> { DOOR_TYPE.RIGHT, DOOR_TYPE.BOTTOM },
            minDoor = 1,
            maxDoor = 3,
            onSpawn = onSpawn,
            randomWalkMin = Params.minSize,
            randomWalkMax = Params.maxSize,
            cornerIntensity = Params.cornerIntensity
        });
        currentRoom = layout;
        Debug.Log(layout.ToAscii());
        onSpawn = false;
        GenerateWallsAndFloors(currentRoom);
        GenerateDoorways();
        GenerateObjects();
        GenerateEntities();

        AStarPather pather = new AStarPather(currentRoom);

        List<PathNode> path = pather.GetPath(new Vector2Int(2, 2), new Vector2Int(1,1));

        foreach (PathNode VARIABLE in path)
        {
            Debug.Log("Path Node: " + VARIABLE.position);
            
        }

        pather.PrintAscii();

        return room;
    }


    private void GenerateObjects()
    {
        Vector2 randPos = GetRandomPosition(_adjacentOffsetX, _adjacentOffsetY);
        switchInstance = Instantiate(switchPrefab, randPos, Quaternion.identity);
        switchInstance.transform.SetParent(room.Holder);
        room.Switch = switchInstance;
    }

    // TOOD: Move to another class with all the Screen and camera stuff.
    public static Vector2 GetRandomPosition(int offsetX, int offsetY)
    {
        // + 2 is the offset for the walls and one extra so it isn't either next to the walls
        float targetHorizontalPos = Random.Range(Const.MapRenderOffsetX + 2, Const.MapWitdth - 1);
        float targetVerticalPos = Random.Range(Const.MapRenderOffsetY + 2, Const.MapHeight - 1);

        return new Vector2(targetHorizontalPos + offsetX, targetVerticalPos + offsetY);
    }

    // Generate the walls and floor of the room, randomazing the various varieties
    void GenerateWallsAndFloors(RoomLayout layout = null)
    {
        if (layout == null)
        {
            GenerateDefaultWallAndFloor();
            return;
        }

        ICell[,] grid = layout.grid;


        for (int y = 0; y < Const.MapHeight; y++)
        {
            for (int x = 0; x < Const.MapWitdth; x++)
            {
                GameObject tile = null;

                ICell cell = grid[x, y];
                if (cell.cellType == CellType.Floor)
                {
                    tile = floorTiles[Random.Range(0, floorTiles.Length)];
                }
                else if (cell.cellType == CellType.Wall)
                {
                    if (cell.direction == DIRECTION.Left)
                    {
                        tile = leftWallsTiles[Random.Range(0, leftWallsTiles.Length)];
                    }
                    else if (cell.direction == DIRECTION.Right)
                    {
                        tile = rightWallsTiles[Random.Range(0, rightWallsTiles.Length)];
                    }
                    else if (cell.direction == DIRECTION.Up)
                    {
                        tile = topWallsTiles[Random.Range(0, topWallsTiles.Length)];
                    }
                    else if (cell.direction == DIRECTION.Down)
                    {
                        tile = bottomWallsTiles[Random.Range(0, bottomWallsTiles.Length)];
                    }
                    else if (cell.direction == DIRECTION.BottomLeftCorner)
                    {
                        tile = bottomLeftCornerTile;
                    }
                    else if (cell.direction == DIRECTION.BottomRightCorner)
                    {
                        tile = bottomRightCornerTile;
                    }
                    else if (cell.direction == DIRECTION.TopLeftCorner)
                    {
                        tile = topLeftCornerTile;
                    }
                    else if (cell.direction == DIRECTION.TopRightCorner)
                    {
                        tile = topRightCornerTile;
                    }

                }

                else if (cell.cellType == CellType.SPAWN)
                {
                    tile = floorTiles[Random.Range(0, floorTiles.Length)];
                }
                else if (cell.cellType == CellType.Void)
                {
                    continue; // Skip void cells
                }
                else
                {
                    continue;
                }

                if (tile == null) continue;
                Vector3 position = new Vector3(x + Const.MapRenderOffsetX + _adjacentOffsetX,
                    y + Const.MapRenderOffsetY + _adjacentOffsetY, 0f);
                GameObject instance = Instantiate(tile, position, Quaternion.identity);
                instance.transform.SetParent(room.Holder);
            }
        }


    }

    void GenerateDefaultWallAndFloor()
    {
        for (int y = 0; y < Const.MapHeight; y++)
        {
            for (int x = 0; x < Const.MapWitdth; x++)
            {
                GameObject tile;

                // Corner tiles
                if (x == 0 && y == 0)
                {
                    tile = bottomLeftCornerTile;
                }
                else if (x == 0 && y == Const.MapHeight - 1)
                {
                    tile = topLeftCornerTile;
                }
                else if (x == Const.MapWitdth - 1 && y == 0)
                {
                    tile = bottomRightCornerTile;
                }
                else if (x == Const.MapWitdth - 1 && y == Const.MapHeight - 1)
                {
                    tile = topRightCornerTile;
                }
                //random left - hand walls, right walls, top, bottom
                else if (x == 0)
                {

                    tile = leftWallsTiles[Random.Range(0, leftWallsTiles.Length)];
                }
                else if (x == Const.MapWitdth - 1)
                {

                    tile = rightWallsTiles[Random.Range(0, rightWallsTiles.Length)];
                }
                else if (y == 0)
                {
                    tile = bottomWallsTiles[Random.Range(0, topWallsTiles.Length)];
                }
                else if (y == Const.MapHeight - 1)
                {
                    tile = topWallsTiles[Random.Range(0, bottomWallsTiles.Length)];
                }
                // if it's not a corner or a wall tile, be it a floor tile
                else
                {
                    tile = floorTiles[Random.Range(0, floorTiles.Length)];
                }

                Vector3 position = new Vector3(x + Const.MapRenderOffsetX + _adjacentOffsetX,
                    y + Const.MapRenderOffsetY + _adjacentOffsetY, 0f);

                GameObject instance = Instantiate(tile, position, Quaternion.identity);

                instance.transform.SetParent(room.Holder);
            }
        }
    }


/// <summary>
/// Converts your DIRECTION enum into the Position enum you use
/// on your DoorwayPrefabs list.
/// </summary>
private Position DirectionToPositionEnum(DIRECTION dir)
{
    switch (dir)
    {
        case DIRECTION.Up:    return Position.TOP;
        case DIRECTION.Down:  return Position.BOTTOM;
        case DIRECTION.Left:  return Position.LEFT;
        case DIRECTION.Right: return Position.RIGHT;
        default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
    }
}


    void GenerateDoorways()
{
    List<ICell> doors = currentRoom.GetDoors();
    if (doors.Count == 0)
    {   
        Debug.LogWarning("No doors found in the room layout.");
        return;
    }
    
    foreach (var door in doors)
    {
        Doorway doorway = null;
        Vector2 doorPosition = WorldPos(door.Position.x, door.Position.y);
        
        switch (door.direction)
        {
            case DIRECTION.Up:
                doorway = Instantiate(DoorwayPrefabs.Find(x => x.position == Position.TOP), room.DoorwayHolder);
                // For top doors, you might need to adjust Y position if the door prefab's pivot is at the bottom
                // Remove the +1 adjustment if your door prefab is already positioned correctly
                doorway.transform.position = new Vector3(doorPosition.x, doorPosition.y, 0f);
                break;

            case DIRECTION.Down:
                doorway = Instantiate(DoorwayPrefabs.Find(x => x.position == Position.BOTTOM), room.DoorwayHolder);
                doorway.transform.position = new Vector3(doorPosition.x, doorPosition.y-1f, 0f);
                break;
                
            case DIRECTION.Left:
                doorway = Instantiate(DoorwayPrefabs.Find(x => x.position == Position.LEFT), room.DoorwayHolder);
                doorway.transform.position = new Vector3(doorPosition.x-1f, doorPosition.y, 0f);
                break;
                
            case DIRECTION.Right:
                doorway = Instantiate(DoorwayPrefabs.Find(x => x.position == Position.RIGHT), room.DoorwayHolder);
                doorway.transform.position = doorPosition;
                break;
        }

        if (doorway == null)
        {
            Debug.LogError("Doorway prefab not found for direction: " + door.direction);
            continue;
        }
       
        room.Doorways.Add(doorway);
    }
}

    void GenerateEntities()
    {
        
        List<ICell> floors = currentRoom.GetCellsOfType(CellType.Floor);




        int enemyCount = EnemyManager.Instance.GetNumberOfEnemies();
        GameManager.Instance.ResetEnemy();
        GameManager.Instance.roomEnemies = enemyCount;

        if (enemyCount <= 0)
        {
            Debug.LogWarning("No enemies to spawn in the room.");
            return;
        }

        enemies = enemyCount;
        for (int i = 0; i < enemyCount; ++i)
        {
            ICell cell = floors[Random.Range(0, floors.Count)];
            if (cell == null || cell.cellType != CellType.Floor)
            {
                Debug.LogWarning("No valid floor cell found for enemy placement.");
                continue;
            }
            Vector2 position = WorldPos(cell.Position.x, cell.Position.y, true);
            GameObject selectedEnemy = EnemyManager.Instance.GetEnemyPrefab();
            GameObject enemyInstance = Instantiate(selectedEnemy, position, Quaternion.identity);

            EnemyStateManager enemyStateManager = enemyInstance.GetComponent<EnemyStateManager>();
            enemyStateManager.Stats.health = EnemyManager.Instance.spawnParameters.DetermineHelth(EnemyManager.Instance.spawnParameters.testWeight);
            enemyInstance.transform.SetParent(room.Holder);
        }

        foreach (var enemy in enemyPrefabs)
        {
            //ICell cell = floors[Random.Range(0, floors.Count)];
            //if (cell == null || cell.cellType != CellType.Floor)
            //{
            //    Debug.LogWarning("No valid floor cell found for enemy placement.");
            //    continue;
            //}
            //Vector2 position = WorldPos(cell.Position.x, cell.Position.y);
            //GameObject selectedEnemy = EnemyManager.Instance.GetEnemyPrefab();
            //GameObject enemyInstance = Instantiate(selectedEnemy, position, Quaternion.identity);
            //enemyInstance.transform.SetParent(room.Holder);

        }
    }


 
    public Vector2 GetSpawnPoint()
    {
        ICell cell = currentRoom.GetSpawn();

        return WorldPos(cell.Position.x, cell.Position.y);
    }

    public Vector2 WorldPos(int x, int y, bool entity=false)
    {
        ICell cell = currentRoom.grid[x, y];
        if (cell.cellType == CellType.Door || entity)
        return new Vector2(cell.Position.x + Const.MapRenderOffsetX + _adjacentOffsetX,
            cell.Position.y + Const.MapRenderOffsetY + _adjacentOffsetY);
        else
            return new Vector2(cell.Position.x + Const.MapRenderOffsetX,
         cell.Position.y + Const.MapRenderOffsetY);
    }
}

