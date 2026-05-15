using UnityEngine;
using Fusion;
using TMPro;

public class FlamePuzzleLavaBucket : NetworkBehaviour
{
    // Shared flame assignment index across all headsets.
    [Networked]
    private int currentFlameIndex { get; set; }

    [Header("Debug")]
    public TMP_Text debugText;

    // =====================================================
    // FIND PLAYER STATE
    // =====================================================

    private FlamePuzzlePlayerState GetPlayerState()
    {
        FlamePuzzleNetworkPlayer[] players =
            FindObjectsOfType<FlamePuzzleNetworkPlayer>();

        foreach (FlamePuzzleNetworkPlayer player in players)
        {
            // Find the local player.
            if (player.Object.HasInputAuthority)
            {
                return player.playerState;
            }
        }

        return null;
    }

    // =====================================================
    // TOUCH EVENT
    // =====================================================

    public void TouchLava()
    {
        debugText.text +=
            "\nTouchLava called";

        RPC_RequestFlame();
    }

    // =====================================================
    // RPC
    // =====================================================

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestFlame()
    {
        debugText.text +=
            "\nRPC started";

        FlamePuzzlePlayerState playerState =
            GetPlayerState();

        if (playerState == null)
        {
            debugText.text +=
                "\nPlayerState NULL";

            return;
        }

        playerState.debugText.text +=
            "\nRPC_RequestFlame called";

        // Prevent duplicate flames.
        if (playerState.hasFlame)
        {
            playerState.debugText.text +=
                "\nAlready has flame";

            return;
        }

        // Prevent overflow.
        if (currentFlameIndex > 2)
        {
            playerState.debugText.text +=
                "\nAll flame colors assigned.";

            return;
        }

        playerState.debugText.text +=
            "\nCurrent Flame Index: " +
            currentFlameIndex;

        switch (currentFlameIndex)
        {
            case 0:

                playerState.AssignFlame(
                    FlamePuzzlePlayerState.FlameColor.Red
                );

                break;

            case 1:

                playerState.AssignFlame(
                    FlamePuzzlePlayerState.FlameColor.Green
                );

                break;

            case 2:

                playerState.AssignFlame(
                    FlamePuzzlePlayerState.FlameColor.Blue
                );

                break;
        }

        currentFlameIndex =
            currentFlameIndex + 1;
    }
}