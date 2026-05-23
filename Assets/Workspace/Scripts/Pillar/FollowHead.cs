using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class FollowHead : NetworkBehaviour
{
    public Vector3 offset;
    public UnityEvent OnBackToPosition;
    public UnityEvent OnLeavePosition;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        transform.position = Camera.main.transform.position + offset;
        var lookDir = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        transform.rotation = Quaternion.LookRotation(lookDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        RPC_OnBackToPosition();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        RPC_OnLeavePosition();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnBackToPosition()
    {
        OnBackToPosition?.Invoke();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnLeavePosition()
    {
        OnLeavePosition?.Invoke();
    }
}
