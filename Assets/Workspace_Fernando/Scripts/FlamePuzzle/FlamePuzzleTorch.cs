using UnityEngine;
using TMPro;
using System.Collections;

public class FlamePuzzleTorch : MonoBehaviour
{
    [Header("Torch Settings")]

    // Which flame color this torch accepts.
    public FlamePuzzlePlayerState.FlameColor requiredColor;

    // Player reference.
    public FlamePuzzlePlayerState playerState;

    // Hidden password text.
    public GameObject passwordText;

    // Torch flame prefab.
    public GameObject torchFlamePrefab;

    // Where the flame appears.
    public Transform flameSpawnPoint;

    // Activation state.
    private bool isActivated = false;

    // Prevent multiple coroutine starts.
    private bool isChecking = false;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        passwordText.SetActive(false);
    }

    // =====================================================
    // TOUCH EVENT
    // =====================================================

    public void TouchTorch()
    {
        // Prevent reactivation.
        if (isActivated)
            return;

        // Prevent duplicate coroutine calls.
        if (isChecking)
            return;

        // Check if player has correct flame.
        if (!playerState.hasFlame)
            return;

        if (playerState.currentFlameColor != requiredColor)
        {
            playerState.debugText.text +=
                "\nWrong flame color.";

            return;
        }

        // Begin hold interaction.
        StartCoroutine(HoldToActivate());
    }

    // =====================================================
    // HOLD TIMER
    // =====================================================

    private IEnumerator HoldToActivate()
    {
        isChecking = true;

        playerState.debugText.text +=
            "\nHolding torch...";

        yield return new WaitForSeconds(1f);

        ActivateTorch();
    }

    // =====================================================
    // ACTIVATE TORCH
    // =====================================================

    private void ActivateTorch()
    {
        isActivated = true;

        playerState.debugText.text +=
            "\nTorch activated.";

        // Reveal password.
        passwordText.SetActive(true);

        // Spawn flame VFX.
        GameObject flame =
            Instantiate(
                torchFlamePrefab,
                flameSpawnPoint.position,
                Quaternion.identity
            );

        // Apply correct color.
        ParticleSystem particleSystem =
            flame.GetComponent<ParticleSystem>();

        var main = particleSystem.main;

        switch (requiredColor)
        {
            case FlamePuzzlePlayerState.FlameColor.Red:
                main.startColor = Color.red;
                break;

            case FlamePuzzlePlayerState.FlameColor.Green:
                main.startColor = Color.green;
                break;

            case FlamePuzzlePlayerState.FlameColor.Blue:
                main.startColor = Color.blue;
                break;
        }
    }
    
}