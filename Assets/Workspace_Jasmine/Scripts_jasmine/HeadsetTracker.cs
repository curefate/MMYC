using UnityEngine;

public class HeadsetTracker : MonoBehaviour
{
    public Transform table;

    // Adjust this manually
    public Vector3 tableOffset =
        new Vector3(0, 0.75f, 0);

    void Update()
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
    }
}