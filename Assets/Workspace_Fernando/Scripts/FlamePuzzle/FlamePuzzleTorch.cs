using UnityEngine;
using Fusion;

public class FlamePuzzleTorch : NetworkBehaviour
{
    public FlamePuzzlePlayerState.FlameColor requiredColor;

    public GameObject litVFX;

    private bool isLit = false;

    public void TouchTorch()
    {
        if (isLit)
            return;

        NetworkObject localPlayerObject =
            Runner.GetPlayerObject(Runner.LocalPlayer);

        if (localPlayerObject == null)
            return;

        FlamePuzzlePlayerState playerState =
            localPlayerObject.GetComponent<FlamePuzzlePlayerState>();

        if (playerState == null)
            return;

        if (!playerState.hasFlame)
            return;

        FlamePuzzlePlayerState.FlameColor playerColor =
            (FlamePuzzlePlayerState.FlameColor)playerState.flameColor;

        if (playerColor != requiredColor)
            return;

        isLit = true;

        if (litVFX != null)
            litVFX.SetActive(true);

        playerState.RemoveFlame();
    }

}