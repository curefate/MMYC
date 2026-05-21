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

    private GameObject leftFlameInstance;

    private GameObject rightFlameInstance;

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

    public void AssignFlame(FlameColor newColor)
    {
        hasFlame = true;

        switch (newColor)
        {
            case FlameColor.Red:
                flameColorIndex = 1;
                break;

            case FlameColor.Green:
                flameColorIndex = 2;
                break;

            case FlameColor.Blue:
                flameColorIndex = 3;
                break;
        }

        debugText.text +=
            "\nFLAME ASSIGNED: " +
            newColor;
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

    private void SpawnFlames()
    {

        if (!Object.HasStateAuthority)
        {
            return;
        }

        debugText.text += "\nSPAWNING FLAMES";
        debugText.text += "\n" + leftHand.name;
        debugText.text += "\n" + rightHand.name;

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

        switch (flameColorIndex)
        {
            case 1:
                flameColor = Color.red;
                break;

            case 2:
                flameColor = Color.green;
                break;

            case 3:
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
        if (
            hasFlame &&
            leftFlameInstance == null &&
            rightFlameInstance == null
        )
        {
            SpawnFlames();
        }

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
    }
}