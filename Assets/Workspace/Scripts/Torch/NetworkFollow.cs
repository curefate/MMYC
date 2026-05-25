using Fusion;
using UnityEngine;

public class NetworkFollow : NetworkBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool absoluteOffset;
    public bool followRotation;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (target != null)
        {
            if (absoluteOffset)
            {
                transform.position = target.position + offset;
            }
            else
            {
                transform.position = target.position + target.forward * offset.z + target.right * offset.x + target.up * offset.y;
            }

            if (followRotation)
            {
                transform.rotation = target.rotation;
            }
        }
    }
}
