using Fusion;
using UnityEngine;

public class FollowHead : NetworkBehaviour
{
    public Vector3 offset;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        transform.position = Camera.main.transform.position + offset;
    }
}
