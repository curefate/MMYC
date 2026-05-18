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
            blueTorch == null ||
            redTorch == null ||
            greenTorch == null
        )
        {
            return;
        }

        // Check all torches.
        if (
            blueTorch.IsActivated() &&
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

        Debug.Log(
            "ALL TORCHES ACTIVATED."
        );

        if (debugText != null)
        {
            debugText.text +=
                "\nALL TORCHES ACTIVATED.";
        }

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
    }
}