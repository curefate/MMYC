using UnityEngine;

public class MovingPillar : MonoBehaviour
{
   [Header("Movement Status")]
    // This lets all players in the multiplayer session know the table is done
    [Networked] public NetworkBool HasReachedEnd { get; set; }

    [Header("MQTT Topic Setup")]
    private string languageTopic = "MMYC/story_language";

    // Call this method RIGHT BEFORE the players start moving the table
    // (e.g., from a UI button, a lobby event, or a game start function)
    public void LockInAndPublishLanguage(string chosenLanguage)
    {
        if (MQTTProcessor.Instance != null)
        {
            // Publish to MQTT right before movement starts
            MQTTProcessor.Instance.PublishMessage(languageTopic, chosenLanguage);
            Debug.Log($"Language choice locked in and published: {chosenLanguage}");
        }
    }

    // Put a Trigger Collider on your empty EndPosition object. 
    // This OnTriggerEnter goes on the Table itself, checking if it hits that EndPosition zone.
    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        if (other.CompareTag("EndPosition"))
        {
            HasReachedEnd = true;
            Debug.Log("Pillar physically arrived at EndPosition. Ready for story audio!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        if (other.CompareTag("EndPosition"))
        {
            HasReachedEnd = false;
        }
    }
}
