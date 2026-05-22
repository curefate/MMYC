using Fusion;
using TMPro;
using UnityEngine;

public class FlamePuzzleLavaBucket : NetworkBehaviour
{
    [Header("Debug")]
    public TMP_Text debugText;

    [Networked]
    public int currentFlameIndex { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            currentFlameIndex = 0;
        }

        debugText.text +=
            "\nBUCKET SPAWNED";
    }

    public void TouchLava()
    {
        debugText.text +=
            "\n--- TOUCH LAVA ---";

        FlamePuzzlePlayerState localPlayer =
            FindFirstObjectByType<FlamePuzzlePlayerState>();

        debugText.text +=
            "\nFOUND PLAYER: " +
            localPlayer.name;

        debugText.text +=
            "\nINPUT AUTH: " +
            localPlayer.Object.HasInputAuthority;

        debugText.text +=
            "\nSTATE AUTH: " +
            localPlayer.Object.HasStateAuthority;

        if (localPlayer == null)
        {
            debugText.text +=
                "\nNO PLAYER FOUND";

            return;
        }

        if (
            !localPlayer.Object.HasInputAuthority
        )
        {
            return;
        }

        debugText.text +=
            "\nREQUESTING FLAME";

        RPC_RequestFlame(
            localPlayer.Object
        );
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestFlame(
        NetworkObject playerObject
    )
    {
        debugText.text +=
            "\nRPC RECEIVED";

        FlamePuzzlePlayerState playerState =
            playerObject.GetComponent<FlamePuzzlePlayerState>();

        if (playerState == null)
        {
            debugText.text +=
                "\nPLAYER STATE NULL";

            return;
        }

        if (playerState.hasFlame)
        {
            debugText.text +=
                "\nPLAYER ALREADY HAS FLAME";

            return;
        }

        if (currentFlameIndex > 2)
        {
            debugText.text +=
                "\nNO COLORS LEFT";

            return;
        }

        playerState.hasFlame = true;

        playerState.flameColorIndex =
            currentFlameIndex;

        playerState.SpawnFlames();

        playerState.ApplyFlameColor();

        currentFlameIndex++;

        debugText.text +=
            "\nNEXT INDEX: " +
            currentFlameIndex;
    }

}