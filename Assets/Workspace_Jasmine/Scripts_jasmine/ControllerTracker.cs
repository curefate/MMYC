using UnityEngine;

public class ControllerTracker : MonoBehaviour
{
    public Transform cube;

    void Update()
    {
        // Get controller local position
        Vector3 localPos =
            OVRInput.GetLocalControllerPosition(
                OVRInput.Controller.RTouch);

        // Get controller local rotation
        Quaternion localRot =
            OVRInput.GetLocalControllerRotation(
                OVRInput.Controller.RTouch);

        // Find camera rig
        OVRCameraRig rig =
            FindFirstObjectByType<OVRCameraRig>();

        // Get tracking space
        Transform trackingSpace = rig.trackingSpace;

        // Convert local controller position to world position
        Vector3 worldPos =
            trackingSpace.TransformPoint(localPos);

        // Convert local rotation to world rotation
        Quaternion worldRot =
            trackingSpace.rotation * localRot;

        // Move table
        Vector3 currentPos = cube.position;

// Only move on X/Z plane
cube.position = new Vector3(
    worldPos.x,
    currentPos.y,
    worldPos.z
);

// Only rotate around Y axis
Vector3 euler = worldRot.eulerAngles;

cube.rotation = Quaternion.Euler(
    0,
    euler.y,
    0
);

        Debug.Log(worldPos);
        Debug.Log("Tracking Active");
    }
}