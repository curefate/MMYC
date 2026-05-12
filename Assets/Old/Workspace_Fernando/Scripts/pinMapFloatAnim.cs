using Fusion;
using UnityEngine;

public class pinMapFloatAnim : NetworkBehaviour
{
    public float amplitude = 0.005f; // smaller movement
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = (Mathf.Sin(Time.time * speed) + 1f) / 2f * amplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_RecalibratePosition(Vector3 pos)
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer.enabled == false)
        {
            renderer.enabled = true;
        }
        startPos = pos;
    }
}