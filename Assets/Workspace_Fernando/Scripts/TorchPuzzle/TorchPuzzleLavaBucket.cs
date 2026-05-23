using Fusion;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TorchPuzzleLavaBucket : NetworkBehaviour
{
    public GameObject[] redFlame;
    public GameObject[] greenFlame;
    public GameObject[] blueFlame;
    public Transform leftHand;
    public Transform rightHand;
    public TorchPuzzleTorch redTorch;
    public TorchPuzzleTorch greenTorch;
    public TorchPuzzleTorch blueTorch;

    public int ownFlameColorIndex { get; private set; } = -1;

    [Networked]
    public int currentColorIndex { get; private set; } = 0;

    private NetworkObject leftFlame;
    private NetworkObject rightFlame;

    public override void FixedUpdateNetwork()
    {
        if (redTorch.isActivated && greenTorch.isActivated && blueTorch.isActivated)
        {
            if (leftFlame && leftFlame.HasStateAuthority)
            {
                Runner.Despawn(leftFlame);
            }

            if (rightFlame && rightFlame.HasStateAuthority)
            {
                Runner.Despawn(rightFlame);
            }
        }
    }

    public void TouchLava()
    {
        if (ownFlameColorIndex != -1) return;

        GiveFlames();

        ownFlameColorIndex = currentColorIndex;

        RPC_AdvanceBucketState();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_AdvanceBucketState()
    {
        currentColorIndex++;
    }

    private void GiveFlames()
    {
        SpawnFlames();
    }

    private void SpawnFlames()
    {
        if (leftFlame != null || rightFlame != null)
            return;

        var flame_left = currentColorIndex switch
        {
            0 => redFlame[0],
            1 => greenFlame[0],
            2 => blueFlame[0],
            _ => null
        };

        var flame_right = currentColorIndex switch
        {
            0 => redFlame[1],
            1 => greenFlame[1],
            2 => blueFlame[1],
            _ => null
        };

        if (flame_left == null || flame_right == null)
            return;

        leftFlame = flame_left.GetComponent<NetworkObject>();
        RPC_SetActive(leftFlame, true);
        leftFlame.RequestStateAuthority();
        leftFlame.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        rightFlame = flame_right.GetComponent<NetworkObject>();
        RPC_SetActive(rightFlame, true);
        rightFlame.RequestStateAuthority();
        rightFlame.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SetActive(NetworkObject obj, bool active)
    {
        obj.gameObject.SetActive(active);
    }

}