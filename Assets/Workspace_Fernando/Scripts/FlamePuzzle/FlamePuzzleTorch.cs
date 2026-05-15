using UnityEngine;
using Fusion;
using TMPro;
using System.Collections;

public class FlamePuzzleTorch : NetworkBehaviour
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
    [Networked]
    private NetworkBool isActivated { get; set; }

    private bool visualsSpawned = false;

    // Prevent multiple coroutine starts.
    private bool isChecking = false;

    // Store current coroutine.
    private Coroutine activationCoroutine;

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
        activationCoroutine =
            StartCoroutine(HoldToActivate());
    }

    // =====================================================
    // STOP TOUCH
    // =====================================================

    public void StopTouchTorch()
    {
        // Ignore if already activated.
        if (isActivated)
            return;

        // Stop coroutine if running.
        if (activationCoroutine != null)
        {
            StopCoroutine(activationCoroutine);

            activationCoroutine = null;
        }

        isChecking = false;
        activationCoroutine = null;

        playerState.debugText.text +=
            "\nTorch hold canceled.";
    }

    // =====================================================
    // HOLD TIMER
    // =====================================================

    private IEnumerator HoldToActivate()
    {
        isChecking = true;

        playerState.debugText.text +=
            "\nHolding torch...";

        yield return new WaitForSeconds(0.4f);

        // Only activate if still checking.
        if (isChecking)
        {
            ActivateTorch();
        }

    }

    // =====================================================
    // ACTIVATE TORCH
    // =====================================================

    private void ActivateTorch()
    {
        isActivated = true;

        playerState.debugText.text +=
            "\nTorch activated.";

        /*

        //####### MOVED ALL OF THIS TO THE RENDER FUSION FUNCTION ###### //

        // Reveal password.
        //passwordText.SetActive(true);

        // Spawn flame VFX.
        GameObject flame =
            Instantiate(
                torchFlamePrefab,
                flameSpawnPoint.position,
                Quaternion.identity
            );
        // Rotate flame upwards.
        flame.transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);

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
        */

    }

    public bool IsActivated()
    {
        return isActivated;
    }

    public override void Render()
    {
        // Spawn visuals only once.
        if (isActivated && !visualsSpawned)
        {
            visualsSpawned = true;

            passwordText.SetActive(true);

            // Spawn flame VFX.
            GameObject flame =
                Instantiate(
                    torchFlamePrefab,
                    flameSpawnPoint.position,
                    Quaternion.identity
                );

            flame.transform.rotation =
                Quaternion.Euler(-90f, 0f, 0f);

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

}