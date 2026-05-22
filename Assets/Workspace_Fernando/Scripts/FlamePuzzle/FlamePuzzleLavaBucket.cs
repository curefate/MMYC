using UnityEngine;
using Fusion;
using TMPro;

public class FlamePuzzleLavaBucket : NetworkBehaviour
{
    [Networked]
    public int currentFlameIndex { get; set; }

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

        if (FlamePuzzleNetworkPlayer.Local == null)
        {
            debugText.text +=
                "\nLOCAL NETWORK PLAYER NULL";

            return;
        }
        //FlamePuzzlePlayerState localPlayer = FindFirstObjectByType<FlamePuzzlePlayerState>();
        FlamePuzzlePlayerState localPlayer = FlamePuzzleNetworkPlayer.Local.playerState;
        debugText.text += "\nLOCAL NETWORK PLAYER FOUND";

        if (localPlayer == null)
        {
            debugText.text +=
                "\nLOCAL PLAYER NULL";

            return;
        }

        if (
            localPlayer.leftFlameInstance != null ||
            localPlayer.rightFlameInstance != null
        )
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

        debugText.text += "\nREQUESTING FLAME RPC";

        localPlayer.SpawnFlames();
        //RPC_RequestFlame();
        RPC_RequestFlame(localPlayer.GetComponent<NetworkObject>());
        
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //[Rpc(RpcSources.All, RpcTargets.All)]
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

        debugText.text +=
            "\nRPC CURRENT INDEX: " +
            currentFlameIndex;

        switch (currentFlameIndex)
        {
            case 0:
                
                debugText.text += "\nASSIGNING RED";
                localPlayer.hasFlame = true;
                localPlayer.RPC_ApplyFlameVisuals(0);
                //localPlayer.AssignFlame(FlamePuzzlePlayerState.FlameColor.Red);

                break;

            case 1:

                debugText.text += "\nASSIGNING GREEN";
                localPlayer.hasFlame = true;
                localPlayer.RPC_ApplyFlameVisuals(1);
                //localPlayer.AssignFlame(FlamePuzzlePlayerState.FlameColor.Green);

                break;

            case 2:

                debugText.text += "\nASSIGNING BLUE";
                localPlayer.hasFlame = true;
                localPlayer.RPC_ApplyFlameVisuals(2);
                //localPlayer.AssignFlame(FlamePuzzlePlayerState.FlameColor.Blue);

                break;
        }

        debugText.text +=
            "\nCURRENT FLAME INDEX BEFORE: " +
            currentFlameIndex;

        currentFlameIndex++;

        debugText.text +=
            "\nCURRENT FLAME INDEX AFTER: " +
            currentFlameIndex;

        debugText.text +=
            "\nFLAME ASSIGNED";

    }

}