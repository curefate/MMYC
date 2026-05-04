using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    public string RoomID;
    public GameObject RoomObj;
    public List<Room> NeighborRooms;

    public UnityEvent OnEnterRoom;
    public UnityEvent OnExitRoom;
}