using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Minimap : NetworkBehaviour
{
    [Networked]
    public string CurrentRoomID { get; private set; } = string.Empty;

    public pinMapFloatAnim MapPin;

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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SwitchRoom(string roomId)
    {
        if (!Object.HasStateAuthority)
            return;

        if (currentRoom != null)
        {
            currentRoom.OnExitRoom?.Invoke();
        }
        CurrentRoomID = roomId;
        if (currentRoom != null)
        {
            currentRoom.OnEnterRoom?.Invoke();
        }
    }

    public override void Spawned()
    {
        foreach (var room in FindObjectsByType<Room>(FindObjectsSortMode.None))
        {
            RegisterRoom(room);
            room.OnEnterRoom.AddListener(() => { MapPin.Rpc_RecalibratePosition(room.Visual.transform.position); });
            foreach (var neighbor in room.NeighborRooms)
            {
                room.OnEnterRoom.AddListener(() => { if (room.IsFinished) neighbor.Rpc_LockRoom(false); });
                room.OnFinishRoom.AddListener(() => neighbor.Rpc_LockRoom(false));
                room.OnFinishRoom.AddListener(() => neighbor.lidDisappear.Rpc_OpenCover());
            }
            room.OnExitRoom.AddListener(() => Rpc_LockAllRooms());
        }
    }
}
