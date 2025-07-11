using UnityEngine;
using System.Collections.Generic;

public class BSPNode
{
    public RectInt bounds;
    public BSPNode leftChild;
    public BSPNode rightChild;
    public RectInt room;
    public bool hasRoom;
    public List<RectInt> hallways;
    
    public BSPNode(RectInt bounds)
    {
        this.bounds = bounds;
        this.hasRoom = false;
        this.hallways = new List<RectInt>();
    }
    
    public bool IsLeaf()
    {
        return leftChild == null && rightChild == null;
    }
    
    public bool Split(int minRoomSize = 6)
    {
        if (!IsLeaf())
            return false;
            
        bool splitHorizontally = Random.Range(0f, 1f) > 0.5f;
        
        if (bounds.width > bounds.height && (float)bounds.width / bounds.height >= 1.25f)
            splitHorizontally = false;
        else if (bounds.height > bounds.width && (float)bounds.height / bounds.width >= 1.25f)
            splitHorizontally = true;
            
        int max = (splitHorizontally ? bounds.height : bounds.width) - minRoomSize;
        if (max <= minRoomSize)
            return false;
            
        int split = Random.Range(minRoomSize, max);
        
        if (splitHorizontally)
        {
            leftChild = new BSPNode(new RectInt(bounds.x, bounds.y, bounds.width, split));
            rightChild = new BSPNode(new RectInt(bounds.x, bounds.y + split, bounds.width, bounds.height - split));
        }
        else
        {
            leftChild = new BSPNode(new RectInt(bounds.x, bounds.y, split, bounds.height));
            rightChild = new BSPNode(new RectInt(bounds.x + split, bounds.y, bounds.width - split, bounds.height));
        }
        
        return true;
    }
    
    public void CreateRoom()
    {
        if (!IsLeaf())
        {
            if (leftChild != null)
                leftChild.CreateRoom();
            if (rightChild != null)
                rightChild.CreateRoom();
                
            if (leftChild != null && rightChild != null)
            {
                CreateHalls(leftChild.GetRoom(), rightChild.GetRoom());
            }
        }
        else
        {
            int roomWidth = Random.Range(bounds.width / 2, bounds.width - 1);
            int roomHeight = Random.Range(bounds.height / 2, bounds.height - 1);
            
            int roomX = Random.Range(1, bounds.width - roomWidth);
            int roomY = Random.Range(1, bounds.height - roomHeight);
            
            room = new RectInt(bounds.x + roomX, bounds.y + roomY, roomWidth, roomHeight);
            hasRoom = true;
        }
    }
    
    public RectInt GetRoom()
    {
        if (hasRoom)
            return room;
        else
        {
            RectInt leftRoom = new RectInt();
            RectInt rightRoom = new RectInt();
            
            if (leftChild != null)
                leftRoom = leftChild.GetRoom();
            if (rightChild != null)
                rightRoom = rightChild.GetRoom();
                
            if (leftRoom.width == 0 && rightRoom.width == 0)
                return new RectInt();
            else if (rightRoom.width == 0)
                return leftRoom;
            else if (leftRoom.width == 0)
                return rightRoom;
            else if (Random.Range(0f, 1f) > 0.5f)
                return leftRoom;
            else
                return rightRoom;
        }
    }
    
    public void CreateHalls(RectInt room1, RectInt room2)
    {
        List<RectInt> hallways = new List<RectInt>();
        
        Vector2Int point1 = new Vector2Int(room1.x + room1.width / 2, room1.y + room1.height / 2);
        Vector2Int point2 = new Vector2Int(room2.x + room2.width / 2, room2.y + room2.height / 2);
        
        if (Random.Range(0f, 1f) > 0.5f)
        {
            hallways.Add(new RectInt(point1.x, point1.y, point2.x - point1.x, 1));
            hallways.Add(new RectInt(point2.x, point1.y, 1, point2.y - point1.y));
        }
        else
        {
            hallways.Add(new RectInt(point1.x, point1.y, 1, point2.y - point1.y));
            hallways.Add(new RectInt(point1.x, point2.y, point2.x - point1.x, 1));
        }
        
        foreach (RectInt hall in hallways)
        {
            RectInt normalizedHall = new RectInt(
                Mathf.Min(hall.x, hall.x + hall.width),
                Mathf.Min(hall.y, hall.y + hall.height),
                Mathf.Abs(hall.width),
                Mathf.Abs(hall.height)
            );
            
            if (normalizedHall.width == 0) normalizedHall.width = 1;
            if (normalizedHall.height == 0) normalizedHall.height = 1;
            
            this.hallways.Add(normalizedHall);
        }
    }
    
    public List<RectInt> GetAllRooms()
    {
        List<RectInt> rooms = new List<RectInt>();
        
        if (hasRoom)
        {
            rooms.Add(room);
        }
        else
        {
            if (leftChild != null)
                rooms.AddRange(leftChild.GetAllRooms());
            if (rightChild != null)
                rooms.AddRange(rightChild.GetAllRooms());
        }
        
        return rooms;
    }
}