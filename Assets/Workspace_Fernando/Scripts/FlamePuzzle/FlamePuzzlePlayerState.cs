using UnityEngine;
using TMPro;
using Fusion;

public class FlamePuzzlePlayerState : NetworkBehaviour
{
    [Header("Flame State")]

    [Networked]
    public NetworkBool hasFlame { get; set; }

    [Networked]
    public int flameColorIndex { get; set; }

    [Header("Debug")]
    public TMP_Text debugText;

    [Header("Hand References")]
    public Transform leftHand;

    public Transform rightHand;

    [Header("Flame VFX")]
    public GameObject flamePrefab;

    public GameObject leftFlameInstance;

    public GameObject rightFlameInstance;

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

    public void UpdateFlameColorOnly()
    {
        ApplyFlameColor();
    }
    
    public void AssignFlame(FlameColor newColor)
    {
        /*
        hasFlame = true;
        //SpawnFlames();

        debugText.text +=
            "\nASSIGNING COLOR INDEX: " +
            flameColorIndex;

        debugText.text +=
            "\nASSIGNING COLOR ENUM: " +
            newColor;

        switch (newColor)
        {
            case FlameColor.Red:
                flameColorIndex = 0;
                debugText.text +=
                    "\nSYNCED INDEX: " +
                    flameColorIndex;
                //UpdateFlameColorOnly();
                break;

            case FlameColor.Green:
                flameColorIndex = 1;
                debugText.text +=
                    "\nSYNCED INDEX: " +
                    flameColorIndex;
                //UpdateFlameColorOnly();
                break;

            case FlameColor.Blue:
                flameColorIndex = 2;
                debugText.text +=
                    "\nSYNCED INDEX: " +
                    flameColorIndex;
                //UpdateFlameColorOnly();
                break;
        }

        ApplyFlameColor();

        debugText.text +=
            "\nFLAME ASSIGNED: " +
            newColor;
        */
    }

    // =====================================================
    // REMOVE FLAME
    // =====================================================

    public void RemoveFlame()
    {
        hasFlame = false;

        flameColorIndex = 0;

        RemoveFlames();

        debugText.text +=
            "\nFLAMES REMOVED";
    }

    // =====================================================
    // SPAWN FLAMES
    // =====================================================

    public void SpawnFlames()
    {
        // Only allow the LOCAL headset rig
        // to spawn visual flames.
        if (
            Camera.main != null &&
            !transform.IsChildOf(Camera.main.transform.root) &&
            Camera.main.transform.root != transform
        )
        {
            return;
        }

        debugText.text += "\nSPAWNING FLAMES";

        // Prevent duplicates.
        if (
            leftFlameInstance != null ||
            rightFlameInstance != null
        )
        {
            return;
        }

        // Safety checks.
        if (flamePrefab == null)
        {
            debugText.text +=
                "\nFlame prefab is NULL.";

            return;
        }

        if (leftHand == null || rightHand == null)
        {
            debugText.text +=
                "\nHand references missing.";

            return;
        }

        debugText.text += "\n" + leftHand.name;
        debugText.text += "\n" + rightHand.name;

        // Spawn left flame.
        leftFlameInstance =
            Instantiate(flamePrefab, leftHand);

        // Spawn right flame.
        rightFlameInstance =
            Instantiate(flamePrefab, rightHand);

        // Left flame offset.
        leftFlameInstance.transform.localPosition =
            new Vector3(0, -0.02f, 0.08f);

        leftFlameInstance.transform.localRotation =
            Quaternion.Euler(-90f, 0f, 0f);

        // Right flame offset.
        rightFlameInstance.transform.localPosition =
            new Vector3(0, -0.02f, 0.08f);

        rightFlameInstance.transform.localRotation =
            Quaternion.Euler(-90f, 0f, 0f);

        debugText.text +=
            "\nFlames spawned.";

        ApplyFlameColor();

    }

    // =====================================================
    // REMOVE FLAMES
    // =====================================================

    private void RemoveFlames()
    {
        if (leftFlameInstance != null)
        {
            Destroy(leftFlameInstance);

            leftFlameInstance = null;
        }

        if (rightFlameInstance != null)
        {
            Destroy(rightFlameInstance);

            rightFlameInstance = null;
        }
    }

    // =====================================================
    // APPLY COLORS
    // =====================================================

    private void ApplyFlameColor()
    {
        Color flameColor = Color.red;

        if (Time.frameCount % 120 == 0)
        {
            debugText.text +=
                "\nAPPLY COLOR INDEX: " +
                flameColorIndex;
        }

        switch (flameColorIndex)
        {
            case 0:
                flameColor = Color.red;
                break;

            case 1:
                flameColor = Color.green;
                break;

            case 2:
                flameColor = Color.blue;
                break;
        }

        SetParticleColor(
            leftFlameInstance,
            flameColor
        );

        SetParticleColor(
            rightFlameInstance,
            flameColor
        );
    }

    // =====================================================
    // PARTICLE COLOR
    // =====================================================

    private void SetParticleColor(
        GameObject flame,
        Color color
    )
    {
        if (flame == null)
            return;

        ParticleSystem particleSystem =
            flame.GetComponent<ParticleSystem>();

        if (particleSystem == null)
        {
            Debug.LogWarning(
                "ParticleSystem missing from flame."
            );

            return;
        }

        var main = particleSystem.main;

        main.startColor = color;
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        /*
        if (
            hasFlame &&
            leftFlameInstance == null &&
            rightFlameInstance == null
        )
        {
            // ONLY local headset spawns visuals.
            if (
                FlamePuzzleNetworkPlayer.Local != null &&
                FlamePuzzleNetworkPlayer.Local.playerState == this
            )
            {
                SpawnFlames();
            }
        }
        */

        // No flames active.
        if (
            leftFlameInstance == null &&
            rightFlameInstance == null
        )
        {
            return;
        }

        // Hand tracking missing.
        if (leftHand == null || rightHand == null)
        {
            return;
        }

        // Left hand visibility.
        if (leftFlameInstance != null)
        {
            bool leftVisible =
                leftHand.position.y >= 0.15f;

            leftFlameInstance.SetActive(leftVisible);

            // Reset rotation.
            leftFlameInstance.transform.rotation =
                Quaternion.Euler(-90f, 0f, 0f);
        }

        // Right hand visibility.
        if (rightFlameInstance != null)
        {
            bool rightVisible =
                rightHand.position.y >= 0.15f;

            rightFlameInstance.SetActive(rightVisible);

            // Reset rotation.
            rightFlameInstance.transform.rotation =
                Quaternion.Euler(-90f, 0f, 0f);
        }

        /*
        if (
            hasFlame &&
            leftFlameInstance == null &&
            rightFlameInstance == null
        )
        {
            SpawnFlames();
        }
        */

        /*
        if (
            flameColorIndex >= 0 &&
            (
                leftFlameInstance != null ||
                rightFlameInstance != null
            )
        )
        {
            ApplyFlameColor();
        }
        */

        //ApplyFlameColor();

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ApplyFlameVisuals(int colorIndex)
    {
        flameColorIndex = colorIndex;

        //SpawnFlames();

        ApplyFlameColor();
    }

}