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
        puzzleCompleted = true;

        debugText.text +=
            "\nALL TORCHES ACTIVATED.";

        // Remove flames from all players.
        foreach (FlamePuzzlePlayerState player in players)
        {
            player.RemoveFlame();
        }
    }
}