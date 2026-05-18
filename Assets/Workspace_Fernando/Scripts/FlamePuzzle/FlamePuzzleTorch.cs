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

    // Prevent duplicate visual spawning.
    private bool visualsSpawned = false;

    // Prevent multiple coroutine starts.
    private bool isChecking = false;

    // Store current coroutine.
    private Coroutine activationCoroutine;

    // Spawned flame instance.
    private GameObject spawnedFlame;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (passwordText != null)
        {
            passwordText.SetActive(false);
        }
    }

    // =====================================================
    // TOUCH EVENT
    // =====================================================

    public void TouchTorch()
    {
        // Fusion safety.
        if (Object == null || !Object.IsValid)
        {
            Debug.LogWarning(
                "Torch not fully spawned yet."
            );
            return;
        }

        // Prevent reactivation.
        if (isActivated)
            return;

        // Prevent duplicate coroutine calls.
        if (isChecking)
            return;

        // Safety.
        if (playerState == null)
        {
            Debug.LogError(
                "Torch missing playerState reference."
            );
            return;
        }

        // Check if player has correct flame.
        if (!playerState.hasFlame)
            return;

        if (
            playerState.currentFlameColor !=
            requiredColor
        )
        {
            if (
                playerState != null &&
                playerState.debugText != null
            )
            {
                playerState.debugText.text +=
                    "\nWrong flame color.";
            }

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
        // Fusion safety.
        if (Object == null || !Object.IsValid)
            return;

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

        if (
            playerState != null &&
            playerState.debugText != null
        )
        {
            playerState.debugText.text +=
                "\nTorch hold canceled.";
        }
    }

    // =====================================================
    // HOLD TIMER
    // =====================================================

    private IEnumerator HoldToActivate()
    {
        isChecking = true;

        if (
            playerState != null &&
            playerState.debugText != null
        )
        {
            playerState.debugText.text +=
                "\nHolding torch...";
        }

        yield return new WaitForSeconds(0.4f);

        // Only activate if still checking.
        if (isChecking)
        {
            ActivateTorch();
        }

        isChecking = false;
        activationCoroutine = null;
    }

    // =====================================================
    // ACTIVATE TORCH
    // =====================================================

    private void ActivateTorch()
    {
        // Fusion safety.
        if (Object == null || !Object.IsValid)
            return;

        isActivated = true;

        if (
            playerState != null &&
            playerState.debugText != null
        )
        {
            playerState.debugText.text +=
                "\nTorch activated.";
        }
    }

    // =====================================================
    // ACTIVATION STATE
    // =====================================================

    public bool IsActivated()
    {
        // Prevent Fusion access errors.
        if (Object == null || !Object.IsValid)
            return false;

        return isActivated;
    }

    // =====================================================
    // RENDER
    // =====================================================

    public override void Render()
    {
        // Fusion safety.
        if (Object == null || !Object.IsValid)
            return;

        // Spawn visuals only once.
        if (isActivated && !visualsSpawned)
        {
            visualsSpawned = true;

            // Reveal password.
            if (passwordText != null)
            {
                passwordText.SetActive(true);
            }

            // Safety checks.
            if (
                torchFlamePrefab == null ||
                flameSpawnPoint == null
            )
            {
                Debug.LogWarning(
                    "Torch visuals missing prefab or spawn point."
                );
                return;
            }

            // Spawn flame VFX.
            spawnedFlame =
                Instantiate(
                    torchFlamePrefab,
                    flameSpawnPoint.position,
                    Quaternion.identity
                );

            // Rotate flame upwards.
            spawnedFlame.transform.rotation =
                Quaternion.Euler(-90f, 0f, 0f);

            // Apply correct color.
            ParticleSystem particleSystem =
                spawnedFlame.GetComponent<ParticleSystem>();

            if (particleSystem == null)
            {
                Debug.LogWarning(
                    "Torch flame missing ParticleSystem."
                );
                return;
            }

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