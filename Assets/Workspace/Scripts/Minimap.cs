using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Minimap : NetworkBehaviour
{
    [Networked]
    public string CurrentRoomID { get; set; } = string.Empty;

    private Dictionary<string, Room> roomDict;
    private Room currentRoom = null;

    public void RegisterRoom(Room room)
    {
        if (roomDict == null)
        {
            roomDict = new Dictionary<string, Room>();
        }

        if (!roomDict.ContainsKey(room.RoomID))
        {
            roomDict[room.RoomID] = room;
        }
        else
        {
            Debug.LogWarning($"Room with ID {room.RoomID} is already registered.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SwitchToRoom(string roomID)
    {
        if (!Object.HasStateAuthority)
            return;

        if (roomID == CurrentRoomID) return;

        if (roomDict.TryGetValue(roomID, out Room newRoom))
        {
            if (newRoom == currentRoom) return;

            if (currentRoom != null)
            {
                currentRoom.OnExitRoom.Invoke();
            }
            currentRoom = newRoom;
            currentRoom.OnEnterRoom.Invoke();
        }
        else
        {
            Debug.LogError($"Room with ID {roomID} not found in Minimap.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_LockAllRooms()
    {
        if (!Object.HasStateAuthority)
            return;

        foreach (var room in roomDict.Values)
        {
            if (room.RoomID == currentRoom.RoomID) return;
            room.IsAccessible = false;
        }
    }
}
