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
[System.Serializable]
public struct GeneratorInfo
{
   [SerializeField] public int Seed;
   [SerializeField] public int RoomCount;
   [SerializeField] public List<DOOR_TYPE> doors;
   [SerializeField] public int minDoor;
   [SerializeField] public int maxDoor;
   [SerializeField] public bool onSpawn;

   [SerializeField] public int randomWalkMin;
   [SerializeField] public int randomWalkMax;
   [SerializeField] public float cornerIntensity;



}
