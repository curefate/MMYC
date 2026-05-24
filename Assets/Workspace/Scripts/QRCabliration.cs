using Fusion;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class QRCabliration : NetworkBehaviour
{
    public Vector3 offset;
    public bool isDone;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (!Object.HasStateAuthority) return;
        if (isDone) return;

        transform.position = trackable.transform.position + offset;
    }
}
