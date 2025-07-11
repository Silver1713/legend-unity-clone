using System.Collections.Generic;
using UnityEngine;

public class BSPDungeon
{
    private BSPNode rootNode;
    private List<RectInt> rooms;
    private int dungeonWidth;
    private int dungeonHeight;
    
    public List<RectInt> Rooms { get => rooms; }
    
    public BSPDungeon(int width = 40, int height = 40)
    {
        dungeonWidth = width;
        dungeonHeight = height;
        GenerateDungeon();
    }
    
    private void GenerateDungeon()
    {
        rootNode = new BSPNode(new RectInt(0, 0, dungeonWidth, dungeonHeight));
        
        List<BSPNode> leaves = new List<BSPNode>();
        leaves.Add(rootNode);
        
        bool didSplit = true;
        while (didSplit)
        {
            didSplit = false;
            List<BSPNode> newLeaves = new List<BSPNode>();
            
            foreach (BSPNode leaf in leaves)
            {
                if (leaf.IsLeaf() && 
                    (leaf.bounds.width > 10 || leaf.bounds.height > 10) &&
                    Random.Range(0f, 1f) > 0.25f)
                {
                    if (leaf.Split())
                    {
                        newLeaves.Add(leaf.leftChild);
                        newLeaves.Add(leaf.rightChild);
                        didSplit = true;
                    }
                    else
                    {
                        newLeaves.Add(leaf);
                    }
                }
                else
                {
                    newLeaves.Add(leaf);
                }
            }
            
            leaves = newLeaves;
        }
        
        rootNode.CreateRoom();
        rooms = rootNode.GetAllRooms();
    }
    
    public RectInt GetRoomAt(int index)
    {
        if (index >= 0 && index < rooms.Count)
            return rooms[index];
        return new RectInt();
    }
    
    public int GetRoomCount()
    {
        return rooms.Count;
    }
    
    public Vector2Int GetRoomCenter(int roomIndex)
    {
        if (roomIndex >= 0 && roomIndex < rooms.Count)
        {
            RectInt room = rooms[roomIndex];
            return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
        }
        return Vector2Int.zero;
    }
    
    public bool IsPositionInRoom(Vector2Int position, int roomIndex)
    {
        if (roomIndex >= 0 && roomIndex < rooms.Count)
        {
            return rooms[roomIndex].Contains(position);
        }
        return false;
    }
}