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
    [Networked]
    public bool IsFinished { get; set; } = false;

    public List<Room> NeighborRooms;

    public GameObject Visual;
    [HideInInspector]
    public InteractableUnityEventWrapper Wrapper;
    [HideInInspector]
    public LidDisappear lidDisappear;

    public UnityEvent OnFinishRoom;
    public UnityEvent OnEnterRoom;
    public UnityEvent OnExitRoom;

    private Vector3 originalVisualPos;

    void Awake()
    {
        if (NeighborRooms == null || NeighborRooms.Count == 0) Debug.LogWarning($"Room {RoomID} has no neighboring rooms assigned.");

        originalVisualPos = Visual.transform.localPosition;

        Wrapper = GetComponentInChildren<InteractableUnityEventWrapper>();
        if (Wrapper == null)
        {
            Debug.LogWarning($"Room {RoomID} has no InteractableUnityEventWrapper assigned.");
        }

        lidDisappear = GetComponentInChildren<LidDisappear>();
        if (lidDisappear == null)
        {
            Debug.LogWarning($"Room {RoomID} has no LidDisappear assigned.");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_LockRoom(bool lockRoom)
    {
        if (!Object.HasStateAuthority) return;

        IsAccessible = !lockRoom;
    }
}