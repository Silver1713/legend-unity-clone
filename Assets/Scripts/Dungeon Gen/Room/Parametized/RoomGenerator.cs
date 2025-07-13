using UnityEngine;

public static class RoomGenerator
{
    public static RoomLayout GenerateRoomLayout(int width, int height, GeneratorInfo info)
    {
        
        RoomLayout layout = new RoomLayout(width, height, info.Seed);
        Random.InitState(info.Seed);
        
        // Fill room with floors
        layout.FillFloor();
       
        layout.PlaceWall();

        layout.LayoutRoom((ROOM_SHAPE)Random.Range(0, 3));

        layout.PlaceDoors(info.minDoor, info.maxDoor);
        

        if (info.onSpawn)
        {
            layout.PlacePlayerSpawn();  
        }
        return layout;
    }
}
