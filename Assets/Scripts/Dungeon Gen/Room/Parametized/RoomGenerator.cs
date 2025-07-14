using UnityEngine;

public static class RoomGenerator
{
    public static RoomLayout GenerateRoomLayout(int width, int height, GeneratorInfo info)
    {
        RoomLayout layout = new RoomLayout(width, height, info.Seed);
        Random.InitState(info.Seed);
        
        // Fill room with floors first (will be overwritten by layout)
        //layout.FillFloor();
       
        // Place initial walls (will be overwritten by layout)
        //layout.PlaceWall();

        // Now choose room shape - include RandomWalk as option
        ROOM_SHAPE selectedShape = (ROOM_SHAPE)Random.Range(0, 5); // Now includes RandomWalk (0-4)
        layout.LayoutRoom(selectedShape);

        // Only place additional doors if not using RandomWalk (which places its own doors)
        //if (selectedShape != ROOM_SHAPE.RandomWalk)
        //{
        //    layout.PlaceDoors(info.minDoor, info.maxDoor);
        //}

        if (true)
        {
            layout.PlacePlayerSpawn();  
        }
        
        return layout;
    }
}