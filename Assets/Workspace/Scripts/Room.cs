using System.Collections.Generic;
using Fusion;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class Room : NetworkBehaviour
{
    [Networked]
    public string RoomID { get; set; } = string.Empty;
    [Networked]
    public bool IsAccessible { get; set; } = false;

    public List<Room> NeighborRooms;

    public GameObject Visual;
    public InteractableUnityEventWrapper Wrapper;

    public UnityEvent OnFinishRoom;
    public UnityEvent OnEnterRoom;
    public UnityEvent OnExitRoom;

    private Vector3 originalVisualPos;

    void Awake()
    {
        if (NeighborRooms == null || NeighborRooms.Count == 0) Debug.LogWarning($"Room {RoomID} has no neighboring rooms assigned.");

        Minimap minimap = FindFirstObjectByType<Minimap>();
        if (minimap != null)
        {
            minimap.RegisterRoom(this);
        }
        else
        {
            Debug.LogWarning($"No Minimap found for Room {RoomID}.");
        }

        originalVisualPos = Visual.transform.localPosition;
    }

    public override void FixedUpdateNetwork()
    {
        if (IsAccessible)
        {
            Visual.transform.localPosition = originalVisualPos;
            Wrapper.enabled = true;
        }
        else
        {
            Visual.transform.localPosition = Vector3.zero;
            Wrapper.enabled = false;
        }
    }
}