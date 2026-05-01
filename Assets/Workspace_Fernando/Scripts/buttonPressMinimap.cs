using Unity.Netcode;
using UnityEngine;
using TMPro;

public class buttonPressMinimap : NetworkBehaviour
{
    [Header("Visual")]
    public Transform buttonVisual;
    public Vector3 pressedOffset = new Vector3(0, -0.05f, 0);
    public float speed = 6f;

    [Header("Debug")]
    public TextMeshProUGUI debugText;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPressed = false;

    void Start()
    {
        if (buttonVisual == null)
        {
            Debug.LogError("ButtonVisual not assigned!");
            return;
        }

        startPos = buttonVisual.localPosition;
        targetPos = startPos + pressedOffset;
    }

    // 🔥 Called by Poke Interactable event
    public void OnButtonPressed()
    {
        if (isPressed) return;

        if (debugText != null)
            debugText.text = "Button Poked";

        PressClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void PressServerRpc()
    {
        PressClientRpc();
    }

    [ClientRpc]
    void PressClientRpc()
    {
        if (isPressed) return;

        isPressed = true;

        if (debugText != null)
            debugText.text = "RPC → moving button";

        StopAllCoroutines();
        StartCoroutine(MoveDown());
    }

    System.Collections.IEnumerator MoveDown()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            buttonVisual.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        buttonVisual.localPosition = targetPos;
    }

    // Optional: reset (useful later)
    public void ResetButton()
    {
        isPressed = false;
        StopAllCoroutines();
        buttonVisual.localPosition = startPos;

        if (debugText != null)
            debugText.text = "Button Reset";
    }
}