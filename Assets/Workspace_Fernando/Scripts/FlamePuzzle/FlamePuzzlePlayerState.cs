using Fusion;
using TMPro;
using UnityEngine;

public class FlamePuzzlePlayerState : NetworkBehaviour
{
    public GameObject flamePrefab;

    public Transform leftHand;
    public Transform rightHand;

    public TextMeshProUGUI debugText;

    [Networked]
    public bool hasFlame { get; set; }

    [Networked]
    public int flameColorIndex { get; set; }

    public GameObject leftFlameInstance;
    public GameObject rightFlameInstance;

    public override void Spawned()
    {
        FlamePuzzleNetworkPlayer.Local =
            GetComponent<FlamePuzzleNetworkPlayer>();

        debugText.text +=
            "\nPLAYER STATE SPAWNED";
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ApplyFlameVisuals(int colorIndex)
    {
        debugText.text +=
            "\nRPC APPLY VISUALS: " +
            colorIndex;

        flameColorIndex = colorIndex;

        debugText.text += "\nCALLING SPAWN";
        SpawnFlames();

        ApplyFlameColor();
    }

    public void SpawnFlames()
    {
        if (
            leftFlameInstance != null ||
            rightFlameInstance != null
        )
        {
            return;
        }

        /*
        if (
            !Object.HasInputAuthority
        )
        {
            return;
        }
        */

        debugText.text +=
            "\nSPAWNING FLAMES";

        leftFlameInstance =
            Instantiate(flamePrefab, leftHand);

        rightFlameInstance =
            Instantiate(flamePrefab, rightHand);

        leftFlameInstance.transform.localPosition =
            new Vector3(0, -0.02f, 0.08f);

        rightFlameInstance.transform.localPosition =
            new Vector3(0, -0.02f, 0.08f);

        leftFlameInstance.transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);

        rightFlameInstance.transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);

        debugText.text +=
            "\nFLAMES SPAWNED";
    }

    public void ApplyFlameColor()
    {
        Color flameColor = Color.gray;

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

        debugText.text +=
            "\nAPPLY COLOR: " +
            flameColorIndex;

        SetParticleColor(
            leftFlameInstance,
            flameColor
        );

        SetParticleColor(
            rightFlameInstance,
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
}