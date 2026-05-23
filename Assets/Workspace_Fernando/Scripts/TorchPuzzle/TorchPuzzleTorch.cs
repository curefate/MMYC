using Fusion;
using TMPro;
using UnityEngine;

public class TorchPuzzleTorch : NetworkBehaviour
{
    [Header("Torch Settings")]
    public int requiredColorIndex;

    [Header("Torch Flame")]
    public GameObject torchFlamePrefab;

    public Transform flameSpawnPoint;

    private GameObject spawnedFlame;

    [Header("Canvas")]
    public GameObject passwordCanvas;

    [Header("Debug")]
    public TMP_Text debugText;

    [Networked]
    private NetworkBool isActivated { get; set; }

    private bool visualsSpawned;

    private void Start()
    {
        debugText =
            GameObject
            .Find("DebugText")
            .GetComponent<TMP_Text>();

        if (passwordCanvas != null)
        {
            passwordCanvas.SetActive(false);
        }
    }

    public void TouchTorch()
    {
        debugText.text +=
            "\n--- TOUCH TORCH ---";

        if (Object == null || !Object.IsValid)
        {
            debugText.text +=
                "\nOBJECT INVALID";

            return;
        }
        if (isActivated)
        {
            debugText.text +=
                "\nTORCH ALREADY ACTIVATED";

            return;
        }

        TorchPuzzleManager torchPuzzleManager =
            FindFirstObjectByType<TorchPuzzleManager>();

        if (torchPuzzleManager == null)
        {
            debugText.text +=
                "\nMANAGER NULL";

            return;
        }

        int playerColor =
            (int)torchPuzzleManager
            .currentFlameColor - 1;

        debugText.text +=
            "\nPLAYER COLOR: " +
            playerColor;

        if (
            playerColor ==
            requiredColorIndex
        )
        {
            debugText.text +=
                "\nCORRECT TORCH";

            RPC_ActivateTorch();
        }
        else
        {
            debugText.text +=
                "\nWRONG COLOR";
        }

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_ActivateTorch()
    {
        isActivated = true;
    }

    public override void Render()
    {
        if (
            isActivated &&
            !visualsSpawned
        )
        {
            visualsSpawned = true;

            if (passwordCanvas != null)
            {
                passwordCanvas.SetActive(true);
            }

            SpawnTorchFlame();
        }
    }

    private void SpawnTorchFlame()
    {
        if (spawnedFlame != null)
        {
            return;
        }

        if (
            torchFlamePrefab == null ||
            flameSpawnPoint == null
        )
        {
            debugText.text +=
                "\nTORCH FLAME REFERENCES NULL";

            return;
        }

        spawnedFlame =
            Instantiate(
                torchFlamePrefab,
                flameSpawnPoint.position,
                Quaternion.identity
            );

        spawnedFlame.transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);

        ApplyTorchColor();

        debugText.text +=
            "\nTORCH FLAME SPAWNED";
    }

    private void ApplyTorchColor()
    {
        Color flameColor = Color.gray;

        switch (requiredColorIndex)
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

        ParticleSystem particle =
            spawnedFlame.GetComponent<ParticleSystem>();

        if (particle == null)
        {
            return;
        }

        ParticleSystem.MainModule main =
            particle.main;

        main.startColor = flameColor;
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}