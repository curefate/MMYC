using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class FollowHead : NetworkBehaviour
{
    public Vector3 offset;

    public UnityEvent OnBackToPosition;
    public UnityEvent OnLeavePosition;

    [Header("Story Audio")]
    public AudioSource storyAudio;
    public AudioClip englishStory;
    public AudioClip swedishStory;

    [Networked] private NetworkBool StoryStarted { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        transform.position = Camera.main.transform.position + offset;

        var lookDir = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        RPC_OnBackToPosition();

        if (!StoryStarted)
        {
            StoryStarted = true;

            string language = "en";

            if (MQTTProcessor.Instance != null)
                language = MQTTProcessor.Instance.StoryLanguage;
                
                  Debug.Log("StateAuthority selected story language: " + language);

            RPC_PlayStoryAudio(language);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        RPC_OnLeavePosition();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnBackToPosition()
    {
        OnBackToPosition?.Invoke();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnLeavePosition()
    {
        OnLeavePosition?.Invoke();
    }

  [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
private void RPC_PlayStoryAudio(string language)
{
    Debug.Log("RPC_PlayStoryAudio received language: " + language);

    if (storyAudio == null)
    {
        Debug.LogError("Story AudioSource is missing!");
        return;
    }

    if (language == "sv" || language == "swedish")
        storyAudio.clip = swedishStory;
    else
        storyAudio.clip = englishStory;

    if (storyAudio.clip == null)
    {
        Debug.LogError("Selected story audio clip is missing!");
        return;
    }

    storyAudio.Play();

    Debug.Log("Playing audio clip: " + storyAudio.clip.name);
}
}