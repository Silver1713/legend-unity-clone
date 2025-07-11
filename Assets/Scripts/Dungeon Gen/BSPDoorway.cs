using UnityEngine;

public class BSPDoorway : Doorway
{
    private int targetRoomIndex;
    public int TargetRoomIndex { get => targetRoomIndex; }
    
    public void Initialize(Position pos, int targetIndex)
    {
        position = pos;
        targetRoomIndex = targetIndex;
        IsOpen = true;
    }
}