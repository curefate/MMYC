using UnityEngine;
using TMPro;

public class FlamePuzzleManager : MonoBehaviour
{
    [Header("Torches")]

    public FlamePuzzleTorch blueTorch;

    public FlamePuzzleTorch redTorch;

    public FlamePuzzleTorch greenTorch;

    [Header("Players")]

    public FlamePuzzlePlayerState[] players;

    [Header("Debug")]

    public TMP_Text debugText;

    // Prevent duplicate completion.
    private bool puzzleCompleted = false;

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // Ignore if already completed.
        if (puzzleCompleted)
            return;

        // Torch safety.
        if (
            redTorch == null ||
            greenTorch == null
        )
        {
            return;
        }

        // TEMP TEST:
        // Only require 2 torches for testing
        if (
            redTorch.IsActivated() &&
            greenTorch.IsActivated()
        )
        {
            CompletePuzzle();
        }
    }

    // =====================================================
    // COMPLETE PUZZLE
    // =====================================================

    private void CompletePuzzle()
    {
        // Prevent duplicates.
        if (puzzleCompleted)
            return;

        puzzleCompleted = true;

        debugText.text += "\nALL TORCHES ACTIVATED.";

        // Safety.
        if (players == null)
            return;

        // Remove flames from all players.
        foreach (FlamePuzzlePlayerState player in players)
        {
            if (player == null)
                continue;

            player.RemoveFlame();
        }

        debugText.text += "\nALL PLAYER FLAMES REMOVED.";

    }
}