using UnityEngine;
using System;
using System.Collections;

public class SharedAnchorManager : MonoBehaviour
{
    IEnumerator Start()
    {
        // Add anchor component
        OVRSpatialAnchor anchor =
            gameObject.AddComponent<OVRSpatialAnchor>();

        // Wait until created
        while (!anchor.Created)
        {
            yield return null;
        }

        Debug.Log("Anchor Created");

        // Wait until localized
        while (!anchor.Localized)
        {
            yield return null;
        }

        Debug.Log("Anchor Localized");

        // Save anchor
        var saveTask = anchor.SaveAnchorAsync();

        while (!saveTask.IsCompleted)
        {
            yield return null;
        }

        if (saveTask.GetResult())
        {
            Debug.Log("Anchor Saved");

            Guid uuid = anchor.Uuid;

            Debug.Log("ANCHOR UUID = " + uuid);
        }
        else
        {
            Debug.Log("Anchor failed to save");
        }
    }
}