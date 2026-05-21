using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class FollowHead : NetworkBehaviour
{
    public Vector3 offset;
    public UnityEvent OnBackToPosition;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        transform.position = Camera.main.transform.position + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        OnBackToPosition?.Invoke();
    }
}
