using System.Collections.Generic;
using UnityEngine;

public class BSPRoom
{
    private readonly Transform roomHolder;
    private readonly Transform doorwayHolder;
    public Transform Holder { get => roomHolder; }
    public Transform DoorwayHolder { get => doorwayHolder; }
    
    private Dictionary<Position, BSPDoorway> doorways;
    public Dictionary<Position, BSPDoorway> Doorways { get => doorways; }
    
    public GameObject Switch { get; set; }
    
    private int id;
    public int ID { get => id; }
    
    private RectInt bounds;
    public RectInt Bounds { get => bounds; }
    
    public bool HasBeenVisited { get; set; }
    
    public BSPRoom(int roomId, RectInt roomBounds)
    {
        id = roomId;
        bounds = roomBounds;
        HasBeenVisited = false;
        
        roomHolder = new GameObject($"BSPRoom_{roomId}").transform;
        
        if (BSPDungeonManager.Instance != null)
        {
            roomHolder.SetParent(BSPDungeonManager.Instance.DungeonHolder);
        }
        
        roomHolder.gameObject.AddComponent<Rigidbody2D>();
        roomHolder.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        roomHolder.gameObject.AddComponent<CompositeCollider2D>();
        
        doorwayHolder = new GameObject("Doorways").transform;
        doorways = new Dictionary<Position, BSPDoorway>();
        
        doorwayHolder.SetParent(roomHolder);
    }
    
    public void AddDoorway(Position position, int targetRoomIndex)
    {
        if (!doorways.ContainsKey(position))
        {
            GameObject doorwayObj = new GameObject($"Doorway_{position}");
            doorwayObj.transform.SetParent(doorwayHolder);
            
            BSPDoorway doorway = doorwayObj.AddComponent<BSPDoorway>();
            doorway.Initialize(position, targetRoomIndex);
            
            doorways.Add(position, doorway);
        }
    }
    
    public void OpenAllDoors(bool open)
    {
        foreach (var doorway in doorways.Values)
        {
            doorway.IsOpen = open;
        }
    }
    
    public void SetPosition(Vector2 position)
    {
        roomHolder.position = position;
    }
}