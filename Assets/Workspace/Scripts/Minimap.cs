using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Minimap : NetworkBehaviour
{
    [Networked]
    public string CurrentRoomID { get; set; } = string.Empty;

    private Dictionary<string, Room> roomDict;
    private Room currentRoom => roomDict.TryGetValue(CurrentRoomID, out var room) ? room : null;

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
    public void Rpc_EnterCurrentRoom()
    {
        if (!Object.HasStateAuthority)
            return;

        if (!currentRoom.IsFinished)
        {
            // If first time entering current room, lock all rooms
            Rpc_LockAllRooms();
        }
        else
        {
            // Or unlock neighbours
            foreach (var neighbor in currentRoom.NeighborRooms)
            {
                neighbor.Rpc_LockRoom(false);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_LockAllRooms()
    {
        if (!Object.HasStateAuthority)
            return;

        foreach (var room in roomDict.Values)
        {
            room.IsAccessible = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_FinishCurrentRoom()
    {
        if (!Object.HasStateAuthority)
            return;

        if (currentRoom != null)
        {
            currentRoom.IsFinished = true;
            currentRoom.OnFinishRoom.Invoke();
        }
    }

    public override void Spawned()
    {
        foreach (var room in FindObjectsByType<Room>(FindObjectsSortMode.None))
        {
            RegisterRoom(room);
            room.OnEnterRoom.AddListener(() => Rpc_EnterCurrentRoom());
            foreach (var neighbor in room.NeighborRooms)
            {
                room.OnFinishRoom.AddListener(() => neighbor.Rpc_LockRoom(false));
                room.OnFinishRoom.AddListener(() => neighbor.lidDisappear.Rpc_OpenCover());
            }
        }
    }
}
