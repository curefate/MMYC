using UnityEngine;
using TMPro;

public class FlamePuzzlePlayerState : MonoBehaviour
{
    // =====================================================
    // PLAYER IDENTIFICATION
    // =====================================================

    [Header("Player Identification")]
    public int playerID;

    // =====================================================
    // FLAME STATE
    // =====================================================

    [Header("Flame State")]
    public bool hasFlame = false;

    public FlameColor currentFlameColor = FlameColor.None;

    // =====================================================
    // DEBUG UI
    // =====================================================

    [Header("Debug")]
    public TMP_Text debugText;
    

    // =====================================================
    // FLAME COLOR ENUM
    // =====================================================

    public enum FlameColor
    {
        None,
        Red,
        Green,
        Blue
    }

    // =====================================================
    // ASSIGN FLAME
    // =====================================================

    public void AssignFlame(FlameColor newColor)
    {
        hasFlame = true;
        currentFlameColor = newColor;

        debugText.text =
            "Player " + playerID +
            " received " + newColor + " flame.";
    }

    // =====================================================
    // REMOVE FLAME
    // =====================================================

    public void RemoveFlame()
    {
        hasFlame = false;
        currentFlameColor = FlameColor.None;

        debugText.text =
            "Player " + playerID +
            " flame removed.";
    }

}