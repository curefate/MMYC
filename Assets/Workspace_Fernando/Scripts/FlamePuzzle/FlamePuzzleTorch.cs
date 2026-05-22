using TMPro;
using UnityEngine;

public class FlamePuzzleTorch : MonoBehaviour
{
    [Header("Torch Settings")]
    public int requiredColorIndex;

    [Header("Canvas")]
    public GameObject passwordCanvas;

    [Header("Debug")]
    public TMP_Text debugText;

    private void Start()
    {
        debugText =
            GameObject
            .Find("DebugText")
            .GetComponent<TMP_Text>();

        if (passwordCanvas != null)
        {
            passwordCanvas.SetActive(false);
        }
    }

    public void TouchTorch()
    {
        debugText.text +=
            "\n--- TOUCH TORCH ---";

        if (FlamePuzzleNetworkPlayer.Local == null)
        {
            debugText.text +=
                "\nLOCAL PLAYER NULL";

            return;
        }

        FlamePuzzlePlayerState playerState =
            FlamePuzzleNetworkPlayer.Local
            .playerState;

        if (playerState == null)
        {
            debugText.text +=
                "\nPLAYER STATE NULL";

            return;
        }

        if (!playerState.hasFlame)
        {
            debugText.text +=
                "\nPLAYER HAS NO FLAME";

            return;
        }

        debugText.text +=
            "\nPLAYER COLOR: " +
            playerState.flameColorIndex;

        if (
            playerState.flameColorIndex ==
            requiredColorIndex
        )
        {
            debugText.text +=
                "\nCORRECT TORCH";

            passwordCanvas.SetActive(true);
        }
        else
        {
            debugText.text +=
                "\nWRONG COLOR";
        }
    }
}