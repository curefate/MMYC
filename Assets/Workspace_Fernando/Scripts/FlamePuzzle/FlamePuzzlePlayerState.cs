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

        debugText.text += "Player " + playerID + " received " + newColor + " flame.";

        SpawnFlames();
    }

    // =====================================================
    // REMOVE FLAME
    // =====================================================

    public void RemoveFlame()
    {
        hasFlame = false;

        currentFlameColor = FlameColor.None;

        debugText.text +=
            "Player " + playerID +
            " flame removed.";

        RemoveFlames();
    }

    // =====================================================
    // SPAWN FLAMES
    // =====================================================

    private void SpawnFlames()
    {
        // Prevent duplicates.
        if (leftFlameInstance != null)
            return;

        // Spawn left hand flame.
        leftFlameInstance = Instantiate(flamePrefab, leftHand);

        debugText.text += "\nLeft Pos: " + leftHand.position.ToString();

        // Spawn right hand flame.
        rightFlameInstance = Instantiate(flamePrefab, rightHand);

        debugText.text += "\nRight Pos: " + rightHand.position.ToString();

        // Offset flames slightly above hands.
        leftFlameInstance.transform.localPosition = new Vector3(0, -0.02f, 0.08f);
        //leftFlameInstance.transform.localRotation = Quaternion.identity;
        //leftFlameInstance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Resets Orientation, to be upwards

        rightFlameInstance.transform.localPosition = new Vector3(0, -0.02f, 0.08f);
        //rightFlameInstance.transform.localRotation = Quaternion.identity;
        //rightFlameInstance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Resets Orientation, to be upwards

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
        }

        if (rightFlameInstance != null)
        {
            Destroy(rightFlameInstance);
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

        SetParticleColor(leftFlameInstance, flameColor);

        SetParticleColor(rightFlameInstance, flameColor);
    }

    // =====================================================
    // PARTICLE COLOR
    // =====================================================

    private void SetParticleColor(GameObject flame, Color color)
    {
        ParticleSystem particleSystem =
            flame.GetComponent<ParticleSystem>();

        var main = particleSystem.main;

        main.startColor = color;
    }

    // =====================================================
    
    // =====================================================

    private void Update()
    {

        // HIDES THE FLAMES IF THE GAME LOOSES THE HAND TRACKING
        if (leftHand.position.y < 0.15f)
        {
            leftFlameInstance.SetActive(false);
        }
        else
        {
            leftFlameInstance.SetActive(true);
        }
        if (rightHand.position.y < 0.15f)
        {
            rightFlameInstance.SetActive(false);
        }
        else
        {
            rightFlameInstance.SetActive(true);
        }

        // RESET THE FLAMES ROTATION TO AVOID UNWANTED ROTATIONS DUE TO HAND MOVEMENTS
        if (leftFlameInstance != null)
        {
            leftFlameInstance.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
        if (rightFlameInstance != null)
        {
            rightFlameInstance.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }

    }

}