using Fusion;
using UnityEngine;

public class NetworkFollow : NetworkBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool followRotation;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (target != null)
        {
            transform.position = target.position + offset;
            if (followRotation)
            {
                transform.rotation = target.rotation;
            }
        }
    }
}
