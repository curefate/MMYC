using UnityEngine;
using TMPro;
using Fusion; //Added to use networkbehavior

//public class TorchPuzzleManager : MonoBehaviour
public class TorchPuzzleManager : NetworkBehaviour
{
    [Header("Hands")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("Flames")]
    public GameObject flamePrefab;

    [Header("Torches")]
    public TorchPuzzleTorch redTorch;
    public TorchPuzzleTorch greenTorch;
    public TorchPuzzleTorch blueTorch;

    [Header("Debug")]
    public TMP_Text debugText;

    private GameObject leftFlame;
    private GameObject rightFlame;

    private bool completionMessageShown;

    public enum FlameColor
    {
        None,
        Red,
        Green,
        Blue
    }

    public FlameColor currentFlameColor =
        FlameColor.None;

    public void GiveFlames(
        FlameColor newColor
    )
    {
        currentFlameColor = newColor;

        SpawnFlames();

        ApplyFlameColor();

        debugText.text +=
            "\nGIVING FLAMES";
    }

    private void Start()
    {
        debugText.text +=
            "\nTORCH PUZZLE READY";
    }

    private void Update()
    {

        if (leftFlame != null)
        {
            bool leftVisible =
                leftHand.position.y >= 0.15f;

            leftFlame.SetActive(leftVisible);

            if (leftVisible)
            {
                leftFlame.transform.rotation =
                    Quaternion.Euler(-90f, 0f, 0f);
            }
        }

        if (rightFlame != null)
        {
            bool rightVisible =
                rightHand.position.y >= 0.15f;

            rightFlame.SetActive(rightVisible);

            if (rightVisible)
            {
                rightFlame.transform.rotation =
                    Quaternion.Euler(-90f, 0f, 0f);
            }
        }

        if (completionMessageShown)
        {
            return;
        }

        if (
            redTorch.IsActivated() &&
            greenTorch.IsActivated() /*&&
            blueTorch.IsActivated()*/
        )
        {
            debugText.text +=
                "\nTURN OFF ALL FLAMES ON HANDS";

            RemoveFlames();

            completionMessageShown = true;
        }
    }

    private void SpawnFlames()
    {
        if (
            leftFlame != null ||
            rightFlame != null
        )
        {
            return;
        }

        //Local Only Spawn
        /*
        leftFlame = Instantiate(flamePrefab, leftHand);
        rightFlame = Instantiate(flamePrefab, rightHand);
        */
        
        //Shared Online Spawn
        debugText.text += "\nRUNNER TPM NULL: " + (Runner == null);

        leftFlame =
            Runner.Spawn(
                flamePrefab,
                leftHand.position,
                Quaternion.identity
            ).gameObject;

        HandFlameFollow leftFollow =
            leftFlame.GetComponent<HandFlameFollow>();

        leftFollow.ownerManager = this;
        leftFollow.isLeftHand = true;

        rightFlame =
            Runner.Spawn(
                flamePrefab,
                rightHand.position,
                Quaternion.identity
            ).gameObject;

        HandFlameFollow rightFollow =
            rightFlame.GetComponent<HandFlameFollow>();

        rightFollow.ownerManager = this;
        rightFollow.isLeftHand = false;

        leftFlame.transform.localPosition =
            new Vector3(0, -0.02f, 0.08f);

        rightFlame.transform.localPosition =
            new Vector3(0, -0.02f, 0.08f);

        leftFlame.transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);

        rightFlame.transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);

    }

    private void ApplyFlameColor()
    {
        Color flameColor = Color.gray;

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
            leftFlame,
            flameColor
        );

        SetParticleColor(
            rightFlame,
            flameColor
        );
    }

    private void SetParticleColor(
        GameObject flameObject,
        Color color
    )
    {
        if (flameObject == null)
            return;

        ParticleSystem particle =
            flameObject.GetComponent<ParticleSystem>();

        if (particle == null)
            return;

        ParticleSystem.MainModule main =
            particle.main;

        main.startColor = color;
    }

    public void RemoveFlames()
    {
        if (leftFlame != null)
        {
            Destroy(leftFlame);
        }

        if (rightFlame != null)
        {
            Destroy(rightFlame);
        }

        currentFlameColor =
            FlameColor.None;

        debugText.text +=
            "\nFLAMES REMOVED";
    }

}