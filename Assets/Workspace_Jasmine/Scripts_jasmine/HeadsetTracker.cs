using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class HeadsetTracker : MonoBehaviour
{
    public Transform table;
    public TextMeshProUGUI debugText;

    // Adjust this manually
    public Vector3 tableOffset = new Vector3(0, 0.75f, 0);

    [Header("Reveal Settings")]
    [Tooltip("Distance threshold in meters (0.3 = 30cm)")]
    [SerializeField] private float triggerDistance = 0.3f;
    
    [Header("Events")]
    public UnityEvent onDistanceReached;
    public UnityEvent onResetTracking; 

    [Header("Target Destination (X)")]
    [Tooltip("The GameObject marking the 'X' final spot")]
    public Transform targetDestination;
    [Tooltip("How close the table needs to be to snap to the target (0.05 = 5cm)")]
    [SerializeField] private float snapDistance = 0.05f;

    private Vector3 _startPosition;
    private bool _hasStartPosition = false;
    private bool _hasTriggeredReveal = false;
    private bool _isSnappedAndLocked = false; // Prevents moving once locked on "X"

    private void Start()
    {
        ResetReveal(); 
        StartCoroutine(UpdateCoordinatesLoop());
    }

    public void ResetReveal()
    {
        _hasStartPosition = false;
        _hasTriggeredReveal = false;
        _isSnappedAndLocked = false;
        _startPosition = Vector3.zero;
        
        if (onResetTracking != null)
        {
            onResetTracking.Invoke();
        }
        
        Debug.Log("Tracking & Room states reset to default.");
    }

    IEnumerator UpdateCoordinatesLoop()
    {
        while (true)
        {
            // If the table successfully reached its final 'X' spot, stop tracking the headset
            if (_isSnappedAndLocked) yield break;

            UpdateCoordinates();
            yield return new WaitForSeconds(0.33f);
        }
    }

    void UpdateCoordinates()
    {
        OVRCameraRig rig = FindFirstObjectByType<OVRCameraRig>();
        if (rig == null) return; 

        Transform headset = rig.centerEyeAnchor;

        // 1. Calculate Target Position from Headset
        Vector3 targetPos = headset.position + headset.rotation * tableOffset;
        Vector3 finalTablePos = new Vector3(targetPos.x, table.position.y, targetPos.z);

        // 2. Check for Snap Destination (X Mark)
        if (targetDestination != null)
        {
            // Check flat distance to the 'X' target
            Vector3 targetNoY = new Vector3(targetDestination.position.x, 0, targetDestination.position.z);
            Vector3 tableNoY = new Vector3(finalTablePos.x, 0, finalTablePos.z);
            
            if (Vector3.Distance(tableNoY, targetNoY) <= snapDistance)
            {
                // Lock it exactly to the X position
                table.position = new Vector3(targetDestination.position.x, table.position.y, targetDestination.position.z);
                table.rotation = Quaternion.Euler(0, targetDestination.rotation.eulerAngles.y, 0);
                
                _isSnappedAndLocked = true;
                if (debugText != null) debugText.text = "Table locked at final destination!";
                Debug.Log("Table snapped to final target destination.");
                return; 
            }
        }

        // 3. Normal tracking movement if not snapped yet
        table.position = finalTablePos;

        if (!_hasStartPosition)
        {
            _startPosition = table.position;
            _hasStartPosition = true;
        }

        Vector3 euler = headset.rotation.eulerAngles;
        table.rotation = Quaternion.Euler(0, euler.y, 0);

        if (debugText != null)
        {
            debugText.text = "\n trying to update the table " + table.position;
        }

        // 4. Distance Trigger Check (30cm)
        if (!_hasTriggeredReveal)
        {
            Vector3 currentPosNoY = new Vector3(table.position.x, 0, table.position.z);
            Vector3 startPosNoY = new Vector3(_startPosition.x, 0, _startPosition.z);
            float distanceMoved = Vector3.Distance(startPosNoY, currentPosNoY);

            if (debugText != null)
            {
                debugText.text += $"\n Distance moved: {distanceMoved:F2}m / {triggerDistance}m";
            }

            if (distanceMoved >= triggerDistance)
            {
                TriggerReveal();
            }
        }
    }

    private void TriggerReveal()
    {
        _hasTriggeredReveal = true;
        if (onDistanceReached != null)
        {
            onDistanceReached.Invoke();
        }
    }
}