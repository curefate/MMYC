using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class FollowHead : NetworkBehaviour
{
    public Vector3 offset;
    public Transform activatePos;
    public UnityEvent OnActivate;
    public List<AudioClip> audioClips;
    [Networked] public bool isActivated { get; private set; }
    private float _timer;
    private bool _atPosition;

    public override void Spawned()
    {
        OnActivate.AddListener(() => GetComponent<AudioSource>().PlayOneShot(audioClips[MQTTProcessor.Instance.Language]));
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        transform.position = Camera.main.transform.position + offset;
        //var lookDir = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        //transform.rotation = Quaternion.LookRotation(lookDir);

        var yPlanePos = new Vector3(activatePos.position.x, 0, activatePos.position.z);
        var distanceOnYPlane = Vector3.Distance(yPlanePos, new Vector3(transform.position.x, 0, transform.position.z));
        Debug.LogError($"Distance on Y Plane: {distanceOnYPlane}");

        if (isActivated) return;

        if (distanceOnYPlane < 0.15f)
        {
            _atPosition = true;
        }
        else
        {
            _atPosition = false;
            _timer = 0;
        }

        if (_atPosition)
        {
            _timer += Runner.DeltaTime;
            if (_timer > 1.5f)
            {
                Rpc_Activate();
            }
        }

        // Cheat
        if (MQTTProcessor.Instance.CheatCode == 1)
        {
            Rpc_Activate();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_Activate()
    {
        isActivated = true;
        OnActivate.Invoke();
    }
}