using UnityEngine;
using System.Collections.Generic;

public class BSPDungeonTest : MonoBehaviour
{
    [SerializeField] private int dungeonWidth = 40;
    [SerializeField] private int dungeonHeight = 40;
    [SerializeField] private bool showDebugGizmos = true;
    
    private BSPDungeon bspDungeon;
    private List<Color> roomColors;
    
    void Start()
    {
        GenerateBSPDungeon();
    }
    
    void GenerateBSPDungeon()
    {
        bspDungeon = new BSPDungeon(dungeonWidth, dungeonHeight);
        
        roomColors = new List<Color>();
        for (int i = 0; i < bspDungeon.GetRoomCount(); i++)
        {
            roomColors.Add(new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f)));
        }
        
        Debug.Log($"BSP Dungeon generated with {bspDungeon.GetRoomCount()} rooms");
        
        for (int i = 0; i < bspDungeon.GetRoomCount(); i++)
        {
            RectInt room = bspDungeon.GetRoomAt(i);
            Vector2Int center = bspDungeon.GetRoomCenter(i);
            Debug.Log($"Room {i}: Position({room.x}, {room.y}), Size({room.width}, {room.height}), Center({center.x}, {center.y})");
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos || bspDungeon == null) return;
        
        for (int i = 0; i < bspDungeon.GetRoomCount(); i++)
        {
            RectInt room = bspDungeon.GetRoomAt(i);
            
            Gizmos.color = roomColors[i];
            
            Vector3 center = new Vector3(room.x + room.width * 0.5f, room.y + room.height * 0.5f, 0);
            Vector3 size = new Vector3(room.width, room.height, 1);
            
            Gizmos.DrawWireCube(center, size);
            
            Vector2Int roomCenter = bspDungeon.GetRoomCenter(i);
            Gizmos.DrawSphere(new Vector3(roomCenter.x, roomCenter.y, 0), 0.5f);
        }
    }
    
    [ContextMenu("Regenerate Dungeon")]
    void RegenerateDungeon()
    {
        GenerateBSPDungeon();
    }
}