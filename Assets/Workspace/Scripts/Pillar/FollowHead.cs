using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class FollowHead : NetworkBehaviour
{
    public Vector3 offset;
    public Transform activatePos;
    public UnityEvent OnActivate;

    [Header("Audio")]
    public AudioSource storyAudioSource;
    public List<AudioClip> audioClips;

    [Networked] public NetworkBool IsActivated { get; private set; }

    private float timer;
    private bool atPosition;

    private void Awake()
    {
        if (storyAudioSource == null)
            storyAudioSource = GetComponent<AudioSource>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (IsActivated) return;

        transform.position = Camera.main.transform.position + offset;

        Vector3 targetFlat = new Vector3(activatePos.position.x, 0, activatePos.position.z);
        Vector3 currentFlat = new Vector3(transform.position.x, 0, transform.position.z);

        float distance = Vector3.Distance(targetFlat, currentFlat);

        if (distance < 0.15f)
        {
            atPosition = true;
            timer += Runner.DeltaTime;

            if (timer > 1.5f)
                Rpc_Activate();
        }
        else
        {
            atPosition = false;
            timer = 0;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_Activate()
    {
        IsActivated = true;
        OnActivate?.Invoke();
        PlayStoryAudio();
    }

    private void PlayStoryAudio()
    {
        if (storyAudioSource == null) return;
        if (audioClips == null || audioClips.Count == 0) return;

        int index = 0;

        if (MQTTProcessor.Instance != null)
            index = Mathf.Abs(MQTTProcessor.Instance.Riddle) % audioClips.Count;

        AudioClip clip = audioClips[index];

        if (clip == null) return;

        storyAudioSource.Stop();
        storyAudioSource.clip = clip;
        storyAudioSource.Play();
    }
}