using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events; // Added for UnityEvent

public class HeadsetTracker : MonoBehaviour
{
    public Transform table;
    public TextMeshProUGUI debugText;

    // Adjust this manually
    public Vector3 tableOffset = new Vector3(0, 0.75f, 0);

    [Header("Reveal Settings")]
    [Tooltip("Distance threshold in meters (0.3 = 30cm)")]
    [SerializeField] private float triggerDistance = 0.3f;
    public UnityEvent onDistanceReached;

    private Vector3 _startPosition;
    private bool _hasStartPosition = false;
    private bool _hasTriggeredReveal = false;

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
        OVRCameraRig rig = FindFirstObjectByType<OVRCameraRig>();
        if (rig == null) return; // Quick safety check if rig isn't found immediately

        Transform headset = rig.centerEyeAnchor;

        // Apply offset
        Vector3 targetPos = headset.position + headset.rotation * tableOffset;

        // Keep table on floor
        table.position = new Vector3(
            targetPos.x,
            table.position.y,
            targetPos.z
        );

        // Capture the initial position AFTER the table has been placed relative to the headset for the first time
        if (!_hasStartPosition)
        {
            _startPosition = table.position;
            _hasStartPosition = true;
            Debug.Log($"Initial Table Position locked: {_startPosition}");
        }

        // Only rotate around Y axis
        Vector3 euler = headset.rotation.eulerAngles;
        table.rotation = Quaternion.Euler(0, euler.y, 0);
        
        Debug.Log("Trying to update the table ");
        Debug.Log(table.position);
        debugText.text = "\n trying to update the table " + table.position;

        // --- DISTANCE CHECK LOGIC ---
        if (!_hasTriggeredReveal)
        {
            // Calculate distance ignoring vertical Y-axis movement (so it's strictly across the room's floor plane)
            Vector3 currentPosNoY = new Vector3(table.position.x, 0, table.position.z);
            Vector3 startPosNoY = new Vector3(_startPosition.x, 0, _startPosition.z);
            float distanceMoved = Vector3.Distance(startPosNoY, currentPosNoY);

            // Append distance tracker info to your text mesh debug panel
            debugText.text += $"\n Distance moved: {distanceMoved:F2}m / {triggerDistance}m";

            if (distanceMoved >= triggerDistance)
            {
                TriggerReveal();
            }
        }

        // Your coordinate update logic here
        Debug.Log("Coordinates updated");
    }

    private void TriggerReveal()
    {
        _hasTriggeredReveal = true;
        
        if (onDistanceReached != null)
        {
            onDistanceReached.Invoke();
        }
        
        Debug.Log("Table moved 30cm or more! Triggering Room Reveal.");
    }
}