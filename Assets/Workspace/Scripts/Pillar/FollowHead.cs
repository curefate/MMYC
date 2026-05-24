using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class FollowHead : NetworkBehaviour
{
    public Vector3 offset;
    public UnityEvent OnBackToPosition;
    public List<AudioClip> audioClips;
    public bool isMultipleActivatable;
    [Networked] public bool isActivated { get; private set; }

    public override void Spawned()
    {
        OnBackToPosition.AddListener(() => GetComponent<AudioSource>().PlayOneShot(audioClips[MQTTProcessor.Instance.Riddle]));
    }

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

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnBackToPosition()
    {
        if (isActivated && !isMultipleActivatable) return;
        OnBackToPosition?.Invoke();
        isActivated = true;
    }
}
