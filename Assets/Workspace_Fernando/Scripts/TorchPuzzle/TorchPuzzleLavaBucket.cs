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
        debugText.text +=
            "\n--- TOUCH LAVA ---";

        if (torchPuzzleManager == null)
        {
            debugText.text +=
                "\nNO TORCH MANAGER";

            return;
        }

        debugText.text +=
            "\nCURRENT INDEX: " +
            currentColorIndex;


        if (
            torchPuzzleManager.currentFlameColor !=
            TorchPuzzleManager.FlameColor.None
        )
        {
            debugText.text +=
                "\nPLAYER ALREADY HAS FLAMES";

            return;
        }

        switch (currentColorIndex)
        {
            case 0:

                debugText.text +=
                    "\nGIVING RED FLAMES";

                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Red
                );

                break;

            case 1:

                debugText.text +=
                    "\nGIVING GREEN FLAMES";

                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Green
                );

                break;

            case 2:

                debugText.text +=
                    "\nGIVING BLUE FLAMES";

                torchPuzzleManager.GiveFlames(
                    TorchPuzzleManager.FlameColor.Blue
                );

                break;

            default:

                debugText.text +=
                    "\nNO COLORS LEFT";

                return;
        }

        debugText.text +=
            "\nCALLING RPC";

        RPC_AdvanceBucketState();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_AdvanceBucketState()
    {
        currentColorIndex++;

        debugText.text +=
            "\nNEXT COLOR INDEX: " +
            currentColorIndex;
    }

}