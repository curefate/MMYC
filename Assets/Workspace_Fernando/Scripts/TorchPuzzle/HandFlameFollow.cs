using UnityEngine;
using Fusion;
using TMPro;

public class HandFlameFollow : NetworkBehaviour
{
    public bool isLeftHand;

    public Transform targetHand;

    public TorchPuzzleManager ownerManager;

    [Header("Debug")]
    public TMP_Text debugText;

    private void Start()
    {
        debugText =
            GameObject
            .Find("DebugText")
            .GetComponent<TMP_Text>();

        FindTargetHand();
    }

    private void FindTargetHand()
    {
        debugText.text += "\nFIND TARGET HAND";

        //debugText.text += "\nOWNER: " + owner;
        if (ownerManager == null)
        {
            return;
        }

        targetHand =
            isLeftHand
            ? ownerManager.leftHand
            : ownerManager.rightHand;
    }

    private void Update()
    {
        if (targetHand == null)
        {
            FindTargetHand();

            return;
        }

        transform.position =
            targetHand.position;

        transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);
    }
}