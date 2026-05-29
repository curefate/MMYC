using System.Linq;
using Fusion;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class QRRelocation : NetworkBehaviour
{
    public Transform root;
    public Vector3 offset;
    public string payload;

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

        if (trackable.MarkerPayloadString != payload)
        {
            Debug.LogError($"QRRelocation trackable payload mismatch: {trackable.MarkerPayloadString} != {payload}");
            return;
        }

        target = trackable.transform;
        var targetRotationOnY = Quaternion.Euler(0, target.rotation.eulerAngles.y, 0);

        Rpc_AlignRoot(target.position, targetRotationOnY);

        isDone = true;

        Rpc_Stop();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_AlignRoot(Vector3 position, Quaternion rotation)
    {
        root.position = position;
        root.rotation = rotation;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_Stop()
    {
        MRUK.Instance.SceneSettings.TrackableAdded.RemoveListener(OnTrackableAdded);
        this.GetComponent<QRCabliration>().enabled = false;
    }
}