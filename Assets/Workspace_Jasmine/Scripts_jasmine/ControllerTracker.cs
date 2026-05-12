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

        // Get tracking space
        Transform trackingSpace =
            FindObjectOfType<OVRCameraRig>().trackingSpace;

        // Convert local position to world position
        Vector3 worldPos =
            trackingSpace.TransformPoint(localPos);

        // Convert local rotation to world rotation
        Quaternion worldRot =
            trackingSpace.rotation * localRot;

        // Move cube
        cube.position = worldPos;
        cube.rotation = worldRot;
    }
}