using UnityEngine;
using Fusion;
using System.Collections.Generic;
using TMPro;

public class FlamePuzzleNetworkPlayer : NetworkBehaviour
{
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

        if (!AllPlayers.Contains(this))
        {
            AllPlayers.Add(this);

            AddDebug(
                "Added to AllPlayers list"
            );
        }

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
            "#1 Input Authority Player: " +
            Object.InputAuthority
        );

        AddDebug(
            "#2 Has Input Authority: " +
            Object.HasInputAuthority
        );

        AddDebug(
            "#3 Has State Authority: " +
            Object.HasStateAuthority
        );

        // TEMP TEST
        /*
        if (Object.HasStateAuthority)
        {
            Local = this;

            AddDebug("LOCAL PLAYER SET");
        }
        */

        if (
            Camera.main != null &&
            Camera.main.transform.root == transform.root
        )
        {
            Local = this;

            AddDebug("LOCAL PLAYER SET");
        }

        if (playerState != null)
        {
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