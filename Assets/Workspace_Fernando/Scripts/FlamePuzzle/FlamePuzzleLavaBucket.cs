using UnityEngine;
using Fusion;
using TMPro;
using System.Collections.Generic;

public class FlamePuzzleLavaBucket : NetworkBehaviour
{
    // Shared flame assignment index across all headsets.
    [Networked]
    private int currentFlameIndex { get; set; }

    [Header("Debug")]
    public TMP_Text debugText;

    // =====================================================
    // SPAWNED
    // =====================================================

    public override void Spawned()
    {
        Debug.Log("Bucket Spawned");

        if (debugText != null)
        {
            debugText.text +=
                "\nBUCKET SPAWNED";

            debugText.text +=
                "\nHas State Authority: " +
                Object.HasStateAuthority;

            debugText.text +=
                "\nHas Input Authority: " +
                Object.HasInputAuthority;
        }

        // Reset only on state authority.
        if (Object != null && Object.HasStateAuthority)
        {
            currentFlameIndex = 0;
        }
    }

    // =====================================================
    // FIND PLAYER STATE
    // =====================================================

    private FlamePuzzlePlayerState
    GetPlayerState(int playerId)
    {
        if (debugText != null)
        {
            debugText.text +=
                "\nSearching Players...";
        }

        List<FlamePuzzleNetworkPlayer> players =
            FlamePuzzleNetworkPlayer.AllPlayers;

        if (debugText != null)
        {
            debugText.text +=
                "\nPlayers Count: " +
                players.Count;
        }

        foreach (FlamePuzzleNetworkPlayer player in players)
        {
            // Safety.
            if (player == null)
                continue;

            if (debugText != null)
            {
                debugText.text +=
                    "\nChecking Player ID: " +
                    player.PlayerID;
            }

            if (player.PlayerID == playerId)
            {
                if (debugText != null)
                {
                    debugText.text +=
                        "\nMATCH FOUND";
                }

                return player.playerState;
            }
        }

        if (debugText != null)
        {
            debugText.text +=
                "\nNO MATCH";
        }

        return null;
    }

    // =====================================================
    // TOUCH EVENT
    // =====================================================

    public void TouchLava()
    {
        if (debugText != null)
        {
            debugText.text +=
                "\n--- TOUCH LAVA ---";

            debugText.text +=
                "\nTouchLava called";
        }

        // Runner safety.
        if (Runner == null)
        {
            if (debugText != null)
            {
                debugText.text +=
                    "\nRunner NULL";
            }

            return;
        }

        // Object safety.
        if (Object == null)
        {
            if (debugText != null)
            {
                debugText.text +=
                    "\nObject NULL";
            }

            return;
        }

        if (!Object.IsValid)
        {
            if (debugText != null)
            {
                debugText.text +=
                    "\nObject INVALID";
            }

            return;
        }

        // Authority debug.
        if (debugText != null)
        {
            debugText.text +=
                "\nHas Input Authority: " +
                Object.HasInputAuthority;

            debugText.text +=
                "\nHas State Authority: " +
                Object.HasStateAuthority;

            debugText.text +=
                "\nRunner Exists: " +
                (Runner != null);

            debugText.text +=
                "\nLocal Player ID: " +
                Runner.LocalPlayer.PlayerId;

            debugText.text +=
                "\nCalling RPC...";
        }

        debugText.text +=
            "\nRPC OBJECT VALID: " +
            (Object != null);

        debugText.text +=
            "\nRPC OBJECT ID: " +
            Object.Id;

        RPC_RequestFlame(
            Runner.LocalPlayer.PlayerId
        );
    }

    // =====================================================
    // RPC
    // =====================================================

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestFlame(int playerId)
    {

        debugText.text +=
            "\nRPC ARRIVED OBJECT ID: " +
            Object.Id;

        if (debugText != null)
        {
            debugText.text +=
                "\nRPC RECEIVED";
        }

        // Fusion safety.
        if (Object == null || !Object.IsValid)
        {
            if (debugText != null)
            {
                debugText.text +=
                    "\nNetwork Object invalid";
            }

            return;
        }

        FlamePuzzlePlayerState playerState =
            GetPlayerState(playerId);

        if (playerState == null)
        {
            if (debugText != null)
            {
                debugText.text +=
                    "\nPlayerState NULL";
            }

            return;
        }

        if (playerState.debugText != null)
        {
            playerState.debugText.text +=
                "\nRPC_RequestFlame called";
        }

        // Prevent duplicate flames.
        if (playerState.hasFlame)
        {
            if (playerState.debugText != null)
            {
                playerState.debugText.text +=
                    "\nAlready has flame";
            }

            return;
        }

        // Prevent overflow.
        if (currentFlameIndex > 2)
        {
            if (playerState.debugText != null)
            {
                playerState.debugText.text +=
                    "\nAll flame colors assigned.";
            }

            return;
        }

        if (playerState.debugText != null)
        {
            playerState.debugText.text +=
                "\nCurrent Flame Index: " +
                currentFlameIndex;
        }

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

        currentFlameIndex++;
    }
}