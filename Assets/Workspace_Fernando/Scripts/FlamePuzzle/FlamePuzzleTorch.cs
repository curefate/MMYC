using UnityEngine;
using Fusion;
using TMPro;
using System.Collections;

public class FlamePuzzleTorch : NetworkBehaviour
{
    [Header("Torch Settings")]

    // Which flame color this torch accepts.
    public FlamePuzzlePlayerState.FlameColor requiredColor;

    // Debug
    private TMP_Text debugText;

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

        debugText =
            GameObject
            .Find("DebugText")
            .GetComponent<TMP_Text>();
    }

    // =====================================================
    // TOUCH EVENT
    // =====================================================

    public void TouchTorch()
    {
        debugText.text +=
            "\n--- TOUCH TORCH ---";

        // Fusion safety.
        if (Object == null || !Object.IsValid)
        {
            debugText.text +=
                "\nTORCH OBJECT INVALID";

            return;
        }

        // Prevent reactivation.
        if (isActivated)
        {
            debugText.text +=
                "\nTORCH ALREADY ACTIVATED";

            return;
        }

        // Prevent duplicate coroutine calls.
        if (isChecking)
        {
            debugText.text +=
                "\nALREADY CHECKING";

            return;
        }

        //FlamePuzzlePlayerState localPlayer = FindFirstObjectByType<FlamePuzzlePlayerState>();
        FlamePuzzlePlayerState localPlayer = FlamePuzzleNetworkPlayer.Local.playerState;

        if (localPlayer == null)
        {
            debugText.text +=
                "\nLOCAL PLAYER NULL";

            return;
        }

        debugText.text +=
            "\nPLAYER FOUND";

        // Check if player has flame.
        if (!localPlayer.hasFlame)
        {
            debugText.text +=
                "\nPLAYER HAS NO FLAME";

            return;
        }

        // Wrong color.
        if (GetRequiredColorIndex() != localPlayer.flameColorIndex)
        //if (localPlayer.currentFlameColor != requiredColor)
        {
            debugText.text +=
                "\nWRONG FLAME COLOR";

            return;
        }

        debugText.text +=
            "\nCORRECT FLAME COLOR";

        // Begin hold interaction.
        activationCoroutine =
            StartCoroutine(
                HoldToActivate()
            );
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

        debugText.text +=
            "\nTORCH HOLD CANCELED";
    }

    // =====================================================
    // HOLD TIMER
    // =====================================================

    private IEnumerator HoldToActivate()
    {
        isChecking = true;

        debugText.text +=
            "\nHOLDING TORCH";

        yield return new WaitForSeconds(0.4f);

        // Only activate if still checking.
        if (isChecking)
        {
            debugText.text +=
                "\nCALLING RPC";

            RPC_RequestActivateTorch();
        }

        isChecking = false;

        activationCoroutine = null;
    }

    // =====================================================
    // RPC
    // =====================================================

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestActivateTorch()
    {
        debugText.text +=
            "\nRPC RECEIVED";

        ActivateTorch();
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

        debugText.text +=
            "\nTORCH ACTIVATED";
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

            debugText.text +=
                "\nSPAWNING TORCH VISUALS";

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
                debugText.text +=
                    "\nTORCH PREFAB OR SPAWN POINT NULL";

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
                debugText.text +=
                    "\nPARTICLE SYSTEM NULL";

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

            debugText.text +=
                "\nTORCH VISUALS SPAWNED";
        }
    }

    private int GetRequiredColorIndex()
    {
        switch (requiredColor)
        {
            case FlamePuzzlePlayerState.FlameColor.Red:
                return 0;

            case FlamePuzzlePlayerState.FlameColor.Green:
                return 1;

            case FlamePuzzlePlayerState.FlameColor.Blue:
                return 2;
        }

        return 0;
    }

}