using UnityEngine;
using TMPro;

public class FlamePuzzlePlayerState : MonoBehaviour
{
    [Header("Player Identification")]
    public int playerID;

    [Header("Flame State")]
    public bool hasFlame = false;

    public FlameColor currentFlameColor = FlameColor.None;

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

        currentFlameColor = newColor;

        if (debugText != null)
        {
            debugText.text +=
                "\nPlayer " +
                playerID +
                " received " +
                newColor +
                " flame.";
        }

        SpawnFlames();
    }

    // =====================================================
    // REMOVE FLAME
    // =====================================================

    public void RemoveFlame()
    {
        hasFlame = false;

        currentFlameColor = FlameColor.None;

        if (debugText != null)
        {
            debugText.text +=
                "\nPlayer " +
                playerID +
                " flame removed.";
        }

        RemoveFlames();
    }

    // =====================================================
    // SPAWN FLAMES
    // =====================================================

    private void SpawnFlames()
    {
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
            Debug.LogError(
                "FlamePuzzlePlayerState: Flame prefab is NULL."
            );
            return;
        }

        if (leftHand == null || rightHand == null)
        {
            Debug.LogWarning(
                "FlamePuzzlePlayerState: Hand references missing."
            );
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

        if (debugText != null)
        {
            debugText.text +=
                "\nFlames spawned.";
        }

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

        switch (currentFlameColor)
        {
            case FlameColor.Red:
                flameColor = Color.red;
                break;

            case FlameColor.Green:
                flameColor = Color.green;
                break;

            case FlameColor.Blue:
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