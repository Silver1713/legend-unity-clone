using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BSPRoomManager : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject[] floorTiles;
    public GameObject[] topWallsTiles;
    public GameObject[] bottomWallsTiles;
    public GameObject[] leftWallsTiles;
    public GameObject[] rightWallsTiles;
    public GameObject topLeftCornerTile;
    public GameObject topRightCornerTile;
    public GameObject bottomLeftCornerTile;
    public GameObject bottomRightCornerTile;
    
    [Header("Room Objects")]
    public GameObject doorwayPrefab;
    public GameObject switchPrefab;
    public GameObject[] enemyPrefabs;
    
    [Header("Generation Settings")]
    [SerializeField] private int minEnemies = 1;
    [SerializeField] private int maxEnemies = 3;
    
    public BSPRoom GenerateRoom(RectInt roomBounds, int roomId, float scale = 1f)
    {
        BSPRoom room = new BSPRoom(roomId, roomBounds);
        
        GenerateWallsAndFloors(room, roomBounds, scale);
        GenerateSwitch(room, roomBounds, scale);
        GenerateEnemies(room, roomBounds, scale);
        
        return room;
    }
    
    private void GenerateWallsAndFloors(BSPRoom room, RectInt bounds, float scale)
    {
        Vector2 roomWorldPos = new Vector2(bounds.x * scale, bounds.y * scale);
        room.SetPosition(roomWorldPos);
        
        for (int y = 0; y < bounds.height; y++)
        {
            for (int x = 0; x < bounds.width; x++)
            {
                GameObject tile = null;
                Vector3 localPosition = new Vector3(x * scale, y * scale, 0);
                
                bool isLeftEdge = (x == 0);
                bool isRightEdge = (x == bounds.width - 1);
                bool isBottomEdge = (y == 0);
                bool isTopEdge = (y == bounds.height - 1);
                
                if (isLeftEdge && isBottomEdge)
                {
                    tile = bottomLeftCornerTile;
                }
                else if (isLeftEdge && isTopEdge)
                {
                    tile = topLeftCornerTile;
                }
                else if (isRightEdge && isBottomEdge)
                {
                    tile = bottomRightCornerTile;
                }
                else if (isRightEdge && isTopEdge)
                {
                    tile = topRightCornerTile;
                }
                else if (isLeftEdge)
                {
                    tile = leftWallsTiles[Random.Range(0, leftWallsTiles.Length)];
                }
                else if (isRightEdge)
                {
                    tile = rightWallsTiles[Random.Range(0, rightWallsTiles.Length)];
                }
                else if (isBottomEdge)
                {
                    tile = bottomWallsTiles[Random.Range(0, bottomWallsTiles.Length)];
                }
                else if (isTopEdge)
                {
                    tile = topWallsTiles[Random.Range(0, topWallsTiles.Length)];
                }
                else
                {
                    tile = floorTiles[Random.Range(0, floorTiles.Length)];
                }
                
                if (tile != null)
                {
                    GameObject instance = Instantiate(tile, room.Holder);
                    instance.transform.localPosition = localPosition;
                }
            }
        }
    }
    
    private void GenerateSwitch(BSPRoom room, RectInt bounds, float scale)
    {
        float minX = bounds.width * 0.2f;
        float maxX = bounds.width * 0.8f;
        float minY = bounds.height * 0.2f;
        float maxY = bounds.height * 0.8f;
        
        Vector3 switchLocalPos = new Vector3(
            Random.Range(minX, maxX) * scale,
            Random.Range(minY, maxY) * scale,
            0
        );
        
        GameObject switchInstance = Instantiate(switchPrefab, room.Holder);
        switchInstance.transform.localPosition = switchLocalPos;
        room.Switch = switchInstance;
    }
    
    private void GenerateEnemies(BSPRoom room, RectInt bounds, float scale)
    {
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemyPrefabs.Length == 0) break;
            
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            
            float minX = bounds.width * 0.15f;
            float maxX = bounds.width * 0.85f;
            float minY = bounds.height * 0.15f;
            float maxY = bounds.height * 0.85f;
            
            Vector3 enemyLocalPos = new Vector3(
                Random.Range(minX, maxX) * scale,
                Random.Range(minY, maxY) * scale,
                0
            );
            
            GameObject enemyInstance = Instantiate(enemyPrefab, room.Holder);
            enemyInstance.transform.localPosition = enemyLocalPos;
        }
    }
    
    public void GenerateDoorway(BSPRoom room, Position position, RectInt bounds, float scale)
    {
        Vector3 doorwayLocalPos = Vector3.zero;
        Quaternion doorwayRotation = Quaternion.identity;
        
        switch (position)
        {
            case Position.TOP:
                doorwayLocalPos = new Vector3(bounds.width * 0.5f * scale, (bounds.height - 1) * scale, 0);
                break;
            case Position.BOTTOM:
                doorwayLocalPos = new Vector3(bounds.width * 0.5f * scale, 0, 0);
                doorwayRotation = Quaternion.Euler(0, 0, 180);
                break;
            case Position.LEFT:
                doorwayLocalPos = new Vector3(0, bounds.height * 0.5f * scale, 0);
                doorwayRotation = Quaternion.Euler(0, 0, 90);
                break;
            case Position.RIGHT:
                doorwayLocalPos = new Vector3((bounds.width - 1) * scale, bounds.height * 0.5f * scale, 0);
                doorwayRotation = Quaternion.Euler(0, 0, -90);
                break;
        }
        
        if (doorwayPrefab != null)
        {
            GameObject doorwayObj = Instantiate(doorwayPrefab, room.DoorwayHolder);
            doorwayObj.transform.localPosition = doorwayLocalPos;
            doorwayObj.transform.localRotation = doorwayRotation;
            
            if (doorwayObj.TryGetComponent<BSPDoorway>(out BSPDoorway bspDoorway))
            {
                room.Doorways[position] = bspDoorway;
            }
        }
    }
}