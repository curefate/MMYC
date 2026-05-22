using TMPro;
using UnityEngine;

public class FlamePuzzleManager : MonoBehaviour
{
    [Header("Torches")]
    public FlamePuzzleTorch redTorch;
    public FlamePuzzleTorch greenTorch;
    public FlamePuzzleTorch blueTorch;

    [Header("Final Canvas")]
    public GameObject finalCanvas;

    [Header("Debug")]
    public TMP_Text debugText;

    private bool redActivated;
    private bool greenActivated;
    private bool blueActivated;

    public void ActivateTorch(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                redActivated = true;
                break;

            case 1:
                greenActivated = true;
                break;

            case 2:
                blueActivated = true;
                break;
        }

        debugText.text +=
            "\nTORCH ACTIVATED: " +
            colorIndex;

        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (
            redActivated &&
            greenActivated &&
            blueActivated
        )
        {
            debugText.text +=
                "\nPUZZLE COMPLETE";

            finalCanvas.SetActive(true);
        }
    }
}