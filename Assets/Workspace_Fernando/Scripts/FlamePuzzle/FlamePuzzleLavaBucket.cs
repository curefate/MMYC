using UnityEngine;
using Fusion;
using TMPro;

public class FlamePuzzleLavaBucket : NetworkBehaviour
{
    [Networked]
    private int currentFlameIndex { get; set; }

    [Header("Debug")]
    public TMP_Text debugText;

    // =====================================================
    // SPAWNED
    // =====================================================

    public override void Spawned()
    {
        if (debugText != null)
        {
            debugText.text +=
                "\nBUCKET SPAWNED";
        }

        if (Object.HasStateAuthority)
        {
            currentFlameIndex = 0;
        }
    }

    // =====================================================
    // TOUCH
    // =====================================================

    public void TouchLava()
    {
        debugText.text +=
            "\n--- TOUCH LAVA ---";

        //FlamePuzzlePlayerState localPlayer = FindFirstObjectByType<FlamePuzzlePlayerState>();
        FlamePuzzlePlayerState localPlayer = FlamePuzzleNetworkPlayer.Local.playerState;

        if (localPlayer == null)
        {
            debugText.text +=
                "\nLOCAL PLAYER NULL";

            return;
        }

        // Prevent duplicate flames.
        if (localPlayer.hasFlame)
        {
            debugText.text +=
                "\nPLAYER ALREADY HAS FLAME";

            return;
        }

        debugText.text +=
            "\nREQUESTING FLAME RPC";

        debugText.text +=
            "\nHAS STATE AUTH: " +
            Object.HasStateAuthority;

        debugText.text +=
            "\nHAS INPUT AUTH: " +
            Object.HasInputAuthority;

        debugText.text +=
            "\nOBJECT VALID: " +
            Object.IsValid;

        //RPC_RequestFlame();
        RPC_RequestFlame(localPlayer.GetComponent<NetworkObject>());
        
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestFlame(NetworkObject playerObject) //RPC_RequestFlame()
    {
        debugText.text += "\nRPC ACTUALLY ARRIVED";

        debugText.text += "\nPLAYER OBJECT: " + playerObject.name;

        FlamePuzzlePlayerState localPlayer = playerObject.GetComponent<FlamePuzzlePlayerState>();
        
        if (localPlayer != null)
        {
            debugText.text +=
                "\nPLAYER STATE FOUND";
        }
        else
        {
            debugText.text +=
                "\nPLAYER STATE NULL";
        }

        // Prevent duplicate flames.
        if (localPlayer.hasFlame)
        {
            debugText.text +=
                "\nPLAYER ALREADY HAS FLAME";

            return;
        }

        // No more colors.
        if (currentFlameIndex > 2)
        {
            debugText.text +=
                "\nNO COLORS LEFT";

            return;
        }

        switch (currentFlameIndex)
        {
            case 0:

                localPlayer.AssignFlame(
                    FlamePuzzlePlayerState.FlameColor.Red
                );

                break;

            case 1:

                localPlayer.AssignFlame(
                    FlamePuzzlePlayerState.FlameColor.Green
                );

                break;

            case 2:

                localPlayer.AssignFlame(
                    FlamePuzzlePlayerState.FlameColor.Blue
                );

                break;
        }

        currentFlameIndex++;

        debugText.text +=
            "\nFLAME ASSIGNED";
    }

}