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
        OnBackToPosition?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        OnLeavePosition?.Invoke();
    }
}
