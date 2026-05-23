using Fusion;
using UnityEngine;
using TMPro;

public class TorchPuzzleLavaBucket : NetworkBehaviour
{
    [Networked]
    public int currentColorIndex { get; set; }

    public TorchPuzzleManager torchPuzzleManager;

    [Header("Debug")]
    public TMP_Text debugText;

    public void TouchLava()
    {
        if (torchPuzzleManager == null)
        {
            debugText.text += "\nNO TORCH MANAGER";

            return;
        }

        switch (currentColorIndex)
        {
            case 0:
                debugText.text += "\nGIVING RED FLAMES";
                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Red
                );

                break;

            case 1:
                debugText.text += "\nGIVING GREEN FLAMES";
                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Green
                );

                break;

            case 2:
                debugText.text += "\nGIVING BLUE FLAMES";
                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Blue
                );

                break;

            default:

                debugText.text += "\nNO COLORS LEFT";
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