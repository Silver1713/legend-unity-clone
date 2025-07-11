using System.Collections.Generic;
using UnityEngine;

public class BSPDungeon
{
    private BSPNode rootNode;
    private List<RectInt> rooms;
    private List<RectInt> hallways;
    private int dungeonWidth;
    private int dungeonHeight;
    
    public List<RectInt> Rooms { get => rooms; }
    public List<RectInt> Hallways { get => hallways; }
    
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
        hallways = GetAllHallways();
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
    
    private List<RectInt> GetAllHallways()
    {
        List<RectInt> allHallways = new List<RectInt>();
        GetHallwaysRecursive(rootNode, allHallways);
        return allHallways;
    }
    
    private void GetHallwaysRecursive(BSPNode node, List<RectInt> hallwaysList)
    {
        if (node == null) return;
        
        if (node.hallways != null)
        {
            hallwaysList.AddRange(node.hallways);
        }
        
        GetHallwaysRecursive(node.leftChild, hallwaysList);
        GetHallwaysRecursive(node.rightChild, hallwaysList);
    }
    
    public bool AreRoomsConnected(int roomIndex1, int roomIndex2)
    {
        if (roomIndex1 < 0 || roomIndex1 >= rooms.Count || 
            roomIndex2 < 0 || roomIndex2 >= rooms.Count)
            return false;
            
        RectInt room1 = rooms[roomIndex1];
        RectInt room2 = rooms[roomIndex2];
        
        foreach (RectInt hallway in hallways)
        {
            bool touchesRoom1 = RectIntersects(hallway, room1);
            bool touchesRoom2 = RectIntersects(hallway, room2);
            
            if (touchesRoom1 && touchesRoom2)
                return true;
        }
        
        return false;
    }
    
    private bool RectIntersects(RectInt rect1, RectInt rect2)
    {
        return rect1.xMin < rect2.xMax && rect1.xMax > rect2.xMin &&
               rect1.yMin < rect2.yMax && rect1.yMax > rect2.yMin;
    }
}