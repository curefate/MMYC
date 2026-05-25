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

    [Header("Table Reference")]
    // Drag your Table GameObject (with the MovingTable script) into this slot
    public MovingTable physicalTable; 

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
        CheckAndPlayStory();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        // Continuously checks while player is inside their designated area
        CheckAndPlayStory();
    }

    private void CheckAndPlayStory()
    {
        if (StoryStarted) return;

        // The player script simply asks the table: "Are you at the end yet?"
        if (physicalTable != null && physicalTable.HasReachedEnd)
        {
            StoryStarted = true;

            string language = "en";
            if (MQTTProcessor.Instance != null)
                language = MQTTProcessor.Instance.StoryLanguage;

            RPC_PlayStoryAudio(language);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        RPC_OnLeavePosition();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnBackToPosition() => OnBackToPosition?.Invoke();

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnLeavePosition() => OnLeavePosition?.Invoke();

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayStoryAudio(string language)
    {
        if (storyAudio == null) return;

        if (language == "sv" || language == "swedish")
            storyAudio.clip = swedishStory;
        else
            storyAudio.clip = englishStory;

        storyAudio.Play();
    }
}