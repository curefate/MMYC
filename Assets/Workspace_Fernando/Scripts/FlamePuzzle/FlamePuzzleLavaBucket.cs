using Fusion;
using UnityEngine;

public class FlamePuzzleLavaBucket : NetworkBehaviour
{
    private int currentFlameIndex = 0;

    public void TouchLava()
    {
        NetworkObject localPlayerObject =
            Runner.GetPlayerObject(Runner.LocalPlayer);

        if (localPlayerObject == null)
            return;

        FlamePuzzlePlayerState playerState =
            localPlayerObject.GetComponent<FlamePuzzlePlayerState>();

        if (playerState == null)
            return;

        RPC_RequestFlame(playerState.Object.InputAuthority);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestFlame(PlayerRef playerRef)
    {
        NetworkObject playerObject = Runner.GetPlayerObject(playerRef);

        if (playerObject == null)
            return;

        FlamePuzzlePlayerState playerState =
            playerObject.GetComponent<FlamePuzzlePlayerState>();

        if (playerState == null)
            return;

        if (playerState.hasFlame)
            return;

        playerState.AssignFlame(
            (FlamePuzzlePlayerState.FlameColor)currentFlameIndex
        );

        currentFlameIndex++;

        if (currentFlameIndex > 2)
            currentFlameIndex = 0;
    }
}