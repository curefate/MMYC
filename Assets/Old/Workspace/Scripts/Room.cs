using System.Collections.Generic;
using Fusion;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class Room : NetworkBehaviour
{
    [Networked]
    public string RoomID { get; private set; } = string.Empty;
    [Networked]
    public bool IsAccessible { get; set; } = false;
    [Networked]
    public bool IsFinished { get; set; } = false;

    public List<Room> NeighborRooms;

    public GameObject Visual;
    [HideInInspector]
    public PokeInteractable interactable;
    [HideInInspector]
    public LidDisappear lidDisappear;

    public UnityEvent OnFinishRoom;
    public UnityEvent OnEnterRoom;
    public UnityEvent OnExitRoom;

    private readonly Vector3 originalVisualPos = new Vector3(0, 0, -0.3f);

    void Awake()
    {
        if (NeighborRooms == null || NeighborRooms.Count == 0) Debug.LogWarning($"Room {RoomID} has no neighboring rooms assigned.");

        interactable = GetComponentInChildren<PokeInteractable>();
        if (interactable == null)
        {
            Debug.LogWarning($"Room {RoomID} has no PokeInteractable assigned.");
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
            interactable.enabled = true;
        }
        else
        {
            Visual.transform.localPosition = Vector3.zero;
            interactable.enabled = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_LockRoom(bool lockRoom)
    {
        if (!Object.HasStateAuthority) return;

        IsAccessible = !lockRoom;
    }
}