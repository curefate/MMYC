using UnityEngine;
using TMPro;

public class FlamePuzzleLavaBucket : MonoBehaviour
{
    // Reference to the player.
    public FlamePuzzlePlayerState playerState;

    // Keeps track of next flame color.
    private int nextFlameIndex = 0;

    // Flame assignment order.
    private FlamePuzzlePlayerState.FlameColor[] flameOrder =
    {
        FlamePuzzlePlayerState.FlameColor.Red,
        FlamePuzzlePlayerState.FlameColor.Green,
        FlamePuzzlePlayerState.FlameColor.Blue
    };

    // This function will be called by Meta events.
    public void TouchLava()
    {
        // Prevent duplicate flames.
        if (playerState.hasFlame)
            return;

        // Prevent overflow.
        if (nextFlameIndex >= flameOrder.Length)
        {
            playerState.debugText.text =
                "All flame colors assigned.";

            return;
        }

        // Assign next flame.
        FlamePuzzlePlayerState.FlameColor assignedColor =
            flameOrder[nextFlameIndex];

        playerState.AssignFlame(assignedColor);

        nextFlameIndex++;
    }
}