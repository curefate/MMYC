using System.Linq;
using Fusion;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class QRRelocation : NetworkBehaviour
{
    public Transform root;
    public Vector3 offset;

    [Networked] private bool isDone { get; set; }

    private Transform target;

    public override void Spawned()
    {
        MRUK.Instance.SceneSettings.TrackableAdded.AddListener(OnTrackableAdded);
        Debug.LogError("QRRelocation spawned");
    }

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        Debug.LogError("QRRelocation trackable added");

        if (trackable == null) return;
        if (isDone) return;

        target = trackable.transform;

        Rpc_AlignRoot(target.position, target.rotation);

        isDone = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_AlignRoot(Vector3 position, Quaternion rotation)
    {
        root.position = position;
        root.rotation = rotation;
    }
}