using Fusion;
using UnityEngine;
using TMPro;

public class TorchPuzzleLavaBucket : NetworkBehaviour
{
    [Networked]
    public int currentColorIndex { get; set; }

    public TorchPuzzleManager torchPuzzleManager;

    public void TouchLava()
    {

        if (torchPuzzleManager == null)
        {
            return;
        }
        if (
            torchPuzzleManager.currentFlameColor !=
            TorchPuzzleManager.FlameColor.None
        )
        {
            return;
        }

        switch (currentColorIndex)
        {
            case 0:

                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Red
                );
                break;

            case 1:
                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Green
                );
                break;

            case 2:
                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Blue
                );
                break;

            default:
                return;
        }

        RPC_AdvanceBucketState();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_AdvanceBucketState()
    {
        currentColorIndex++;
    }

}