using UnityEngine;
using Fusion;
using System.Collections.Generic;
using TMPro;

public class FlamePuzzleNetworkPlayer : NetworkBehaviour
{
    [Networked]
    public int PlayerID { get; set; }

    [Header("References")]
    public FlamePuzzlePlayerState playerState;

    [Header("Debug")]
    public TMP_Text debugText;

    public static FlamePuzzleNetworkPlayer Local;

    public static List<FlamePuzzleNetworkPlayer> AllPlayers =
        new List<FlamePuzzleNetworkPlayer>();

    // =====================================================
    // SPAWNED
    // =====================================================

    public override void Spawned()
    {
        AddDebug("NETWORK PLAYER SPAWNED");

        // Prevent duplicates.
        if (!AllPlayers.Contains(this))
        {
            AllPlayers.Add(this);

            AddDebug(
                "Added to AllPlayers list"
            );
        }

        // Safety checks.
        if (Runner == null)
        {
            AddDebug("RUNNER NULL");
            return;
        }

        if (Object == null)
        {
            AddDebug("NETWORK OBJECT NULL");
            return;
        }

        AddDebug(
            "Has Input Authority: " +
            Object.HasInputAuthority
        );

        AddDebug(
            "Has State Authority: " +
            Object.HasStateAuthority
        );

        // Store local reference.
        if (Object.HasInputAuthority)
        {
            Local = this;

            AddDebug("LOCAL PLAYER SET");
        }

        // Assign player id.
        PlayerID = Runner.LocalPlayer.PlayerId;

        AddDebug(
            "Assigned Player ID: " +
            PlayerID
        );

        // Sync to player state.
        if (playerState != null)
        {
            playerState.playerID = PlayerID;

            AddDebug(
                "PlayerState linked"
            );
        }
        else
        {
            AddDebug(
                "PlayerState NULL"
            );
        }
    }

    // =====================================================
    // DESTROY
    // =====================================================

    private void OnDestroy()
    {
        if (AllPlayers.Contains(this))
        {
            AllPlayers.Remove(this);
        }

        if (Local == this)
        {
            Local = null;
        }

        AddDebug(
            "NETWORK PLAYER DESTROYED"
        );
    }

    // =====================================================
    // DEBUG
    // =====================================================

    private void AddDebug(string message)
    {
        Debug.Log(message);

        // Use assigned debug text first.
        if (debugText != null)
        {
            debugText.text +=
                "\n" + message;

            return;
        }

        // Fallback to playerState debug text.
        if (
            playerState != null &&
            playerState.debugText != null
        )
        {
            playerState.debugText.text +=
                "\n" + message;
        }
    }
}