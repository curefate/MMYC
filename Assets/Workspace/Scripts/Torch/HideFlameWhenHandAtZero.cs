using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkFollow))]
public class HideFlameWhenHandAtZero : NetworkBehaviour
{
    private ParticleSystem flameParticleSystem;
    private NetworkFollow networkFollow;

    public override void Spawned()
    {
        base.Spawned();
        flameParticleSystem = GetComponent<ParticleSystem>();
        networkFollow = GetComponent<NetworkFollow>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!Object.HasStateAuthority) return;

        if (networkFollow != null && flameParticleSystem != null)
        {
            flameParticleSystem.gameObject.SetActive(networkFollow.target.position != Vector3.zero);
        }
    }
}
