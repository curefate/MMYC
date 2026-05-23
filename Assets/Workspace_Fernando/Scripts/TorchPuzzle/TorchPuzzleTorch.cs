using Fusion;
using TMPro;
using UnityEngine;

public class TorchPuzzleTorch : NetworkBehaviour
{
    public TorchPuzzleLavaBucket lavaBucket;
    [Header("Torch Settings")]
    public int requiredColorIndex;

    [Header("Torch Flame")]
    public GameObject torchFlamePrefab;

    public Transform flameSpawnPoint;

    private GameObject spawnedFlame;

    [Header("Canvas")]
    public GameObject passwordCanvas;

    [Networked]
    public NetworkBool isActivated { get; private set; }

    private bool visualsSpawned;

    private void Start()
    {
        if (passwordCanvas != null)
        {
            passwordCanvas.SetActive(false);
        }
    }

    public void TouchTorch()
    {
        if (Object == null || !Object.IsValid)
        {
            return;
        }
        if (isActivated)
        {
            return;
        }

        int playerColor = lavaBucket.ownFlameColorIndex;

        if (
            playerColor ==
            requiredColorIndex
        )
        {
            RPC_ActivateTorch();
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
}