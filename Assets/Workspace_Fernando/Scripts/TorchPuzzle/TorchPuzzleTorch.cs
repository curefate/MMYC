using TMPro;
using UnityEngine;

public class TorchPuzzleTorch : MonoBehaviour
{
    [Header("Torch Settings")]
    public int requiredColorIndex;

    [Header("Canvas")]
    public GameObject passwordCanvas;

    [Header("Debug")]
    public TMP_Text debugText;

    [HideInInspector]
    public bool isActivated;

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

        TorchPuzzleManager torchPuzzleManager =
            FindFirstObjectByType<TorchPuzzleManager>();

        if (torchPuzzleManager == null)
        {
            debugText.text +=
                "\nMANAGER NULL";

            return;
        }

        int playerColor =
            (int)torchPuzzleManager
            .currentFlameColor - 1;

        debugText.text +=
            "\nPLAYER COLOR: " +
            playerColor;

        if (
            playerColor ==
            requiredColorIndex
        )
        {
            debugText.text +=
                "\nCORRECT TORCH";

            isActivated = true;

            passwordCanvas.SetActive(true);
        }
        else
        {
            debugText.text +=
                "\nWRONG COLOR";
        }
    }
}