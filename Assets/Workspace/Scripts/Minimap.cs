using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Minimap : NetworkBehaviour
{
    private Dictionary<string, Room> roomDict;
    private Room currentRoom;
    public static readonly Vector3 backUpPosition = new Vector3(10, 10, 10);

    public void RegisterRoom(Room room)
    {
        if (!roomDict.ContainsKey(room.RoomID))
        {
            roomDict[room.RoomID] = room;
        }
    }

    private void SwitchToRoom(string roomID)
    {

    }
}
