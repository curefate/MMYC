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
        cube.position = worldPos;
        cube.rotation = worldRot;

        Debug.Log(worldPos);
        Debug.Log("Tracking Active");
    }
}