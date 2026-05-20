using UnityEngine;
using TMPro;
using System.Collections;


public class HeadsetTracker : MonoBehaviour
{
    public Transform table;
    public TextMeshProUGUI debugText;

    // Adjust this manually
    public Vector3 tableOffset =
        new Vector3(0, 0.75f, 0);

     private void Start()
    {
        StartCoroutine(UpdateCoordinatesLoop());
    }

    IEnumerator UpdateCoordinatesLoop()
    {
        while (true)
        {
            UpdateCoordinates();

            yield return new WaitForSeconds(0.33f);
        }
    }

    void UpdateCoordinates()
    {
            OVRCameraRig rig =
            FindFirstObjectByType<OVRCameraRig>();

        Transform headset =
            rig.centerEyeAnchor;

        // Apply offset
        Vector3 targetPos =
            headset.position +
            headset.rotation * tableOffset;

        // Keep table on floor
        table.position = new Vector3(
            targetPos.x,
            table.position.y,
            targetPos.z
        );

        // Only rotate around Y axis
        Vector3 euler =
            headset.rotation.eulerAngles;

        table.rotation =
            Quaternion.Euler(
                0,
                euler.y,
                0
            );
            Debug.Log("Trying to update the table ");
            Debug.Log(table.position);
            debugText.text = "/n trying to update the table " + table.position;
        // Your coordinate update logic here
        Debug.Log("Coordinates updated");
    }

  }