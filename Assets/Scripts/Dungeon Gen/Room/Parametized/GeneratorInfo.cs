using System.Collections.Generic;
using UnityEngine;

public enum DOOR_TYPE
{
    NONE,
    TOP,
    BOTTOM,
    LEFT,
    RIGHT
}
public struct GeneratorInfo
{
    public int Seed;
    public int RoomCount;
    public List<DOOR_TYPE> doors;
    public int minDoor;
    public int maxDoor;
    public bool onSpawn;

}
