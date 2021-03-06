﻿using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
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

    public List<Doorway> DoorwayPrefabs;

    public GameObject switchPrefab;

    // A list of possible locations to place tiles.
    //private List<Vector3> gridPositions = new List<Vector3>();

    private int _adjacentOffsetX;
    private int _adjacentOffsetY;

    private Room room;
    private GameObject switchInstance;

    public Room GenerateRoom(int offsetX, int offsetY)
    {
        _adjacentOffsetX = offsetX;
        _adjacentOffsetY = offsetY;

        room = new Room();

        GenerateWallsAndFloors();
        GenerateDoorways();
        GenerateObjects();

        return room;
    }

    private void Update()
    {
        // Debug: reposition switch when "R" pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (switchInstance != null)
            {
                switchInstance.transform.position = GetRandomPosition();
            }
        }
    }

    private void GenerateObjects()
    {
        Vector2 randPos = GetRandomPosition();
        Vector2 offset = new Vector2(_adjacentOffsetX, _adjacentOffsetY);
        switchInstance = Instantiate(switchPrefab, randPos + offset, Quaternion.identity);
        switchInstance.transform.SetParent(room.Holder);
    }

    public static Vector2 GetRandomPosition()
    {
        // + 1 is the offset for the walls
        float targetHorizontalPos = Random.Range(Const.MapRenderOffsetX + 1 , Const.MapWitdth);
        float targetVerticalPos = Random.Range(Const.MapRenderOffsetY + 1, Const.MapHeight);

        return new Vector2(targetHorizontalPos, targetVerticalPos);
    }

    // Generate the walls and floor of the room, randomizing the various varieties
    void GenerateWallsAndFloors()
    {
        for (int y = 0; y < Const.MapHeight; y++)
        {
            for (int x = 0; x < Const.MapWitdth; x++)
            {
                GameObject toInstantiate;

                // Corner tiles
                if (x == 0 && y == 0)
                {
                    toInstantiate = bottomLeftCornerTile;
                }
                else if (x == 0 && y == Const.MapHeight - 1)
                {
                    toInstantiate = topLeftCornerTile;
                }
                else if (x == Const.MapWitdth - 1 && y == 0)
                {
                    toInstantiate = bottomRightCornerTile;
                }
                else if (x == Const.MapWitdth - 1 && y == Const.MapHeight - 1)
                {
                    toInstantiate = topRightCornerTile;
                }
                //random left - hand walls, right walls, top, bottom
                else if (x == 0)
                {

                    toInstantiate = leftWallsTiles[Random.Range(0, leftWallsTiles.Length)];
                }
                else if (x == Const.MapWitdth - 1)
                {

                    toInstantiate = rightWallsTiles[Random.Range(0, rightWallsTiles.Length)];
                }
                else if (y == 0)
                {
                    toInstantiate = bottomWallsTiles[Random.Range(0, topWallsTiles.Length)];
                }
                else if (y == Const.MapHeight - 1)
                {
                    toInstantiate = topWallsTiles[Random.Range(0, bottomWallsTiles.Length)];
                }
                // if it's not a corner or a wall tile, be it a floor tile
                else
                {
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                }

                Vector3 position = new Vector3(x + Const.MapRenderOffsetX + _adjacentOffsetX, 
                    y + Const.MapRenderOffsetY + _adjacentOffsetY, 0f);

                GameObject instance = Instantiate(toInstantiate, position, Quaternion.identity);

                instance.transform.SetParent(room.Holder);
            }
        }
    }
    void GenerateDoorways()
    {
        foreach (var doorway in DoorwayPrefabs)
        {
            Doorway instance = Instantiate(doorway);
            instance.OffsetDoorway(_adjacentOffsetX, _adjacentOffsetY);
            instance.transform.SetParent(room.DoorwayHolder);

            room.Doorways.Add(instance);
        }
    }
}

public class Room
{
    private readonly Transform roomHolder;
    private readonly Transform doorwayHolder;
    public Transform Holder { get => roomHolder; }
    public Transform DoorwayHolder { get => doorwayHolder; }
    private List<Doorway> doorways;
    public List<Doorway> Doorways { get => doorways; set => doorways = value; }

    public Room()
    {
        roomHolder = new GameObject("Room").transform;
        doorwayHolder = new GameObject("Doorways").transform;
        doorways = new List<Doorway>();
        
        doorwayHolder.SetParent(roomHolder);
    }

    public void OpenAllDoors(bool open)
    {
        foreach(var doorway in doorways)
        {
            doorway.IsOpen = open;
        }
    }

    public void DebugDoorStatus()
    {
        foreach (var doorway in doorways)
        {
            Debug.Log("Doorway " + doorway.Direction + " " + doorway.IsOpen);
        }
    }
}
