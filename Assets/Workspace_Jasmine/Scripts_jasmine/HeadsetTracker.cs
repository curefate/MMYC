using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using Fusion; // REQUIRED FOR MULTIPLAYER SYNC

// Change MonoBehaviour to NetworkBehaviour so it works with Photon Fusion
public class HeadsetTracker : NetworkBehaviour
{
    public Transform table;
    public TextMeshProUGUI debugText;

    public Vector3 tableOffset = new Vector3(0, 0.75f, 0);

    [Header("Reveal Settings")]
    [SerializeField] private float targetDistance = 0.3f;
    
    [Header("Events")]
    public UnityEvent onDistanceReached;
    public UnityEvent onResetTracking; 

    [Header("Target Destination (X)")]
    public Transform targetDestination;
    [SerializeField] private float snapDistance = 0.05f;

    private Vector3 _startPosition;
    private bool _hasStartPosition = false;
    private bool _hasTriggeredReveal = false;
    private bool _isSnappedAndLocked = false; 

    // Spawned() runs instead of Start() in Photon Fusion when the object initializes online
    public override void Spawned()
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
        
        if (targetDestination != null)
        {
            targetDestination.gameObject.SetActive(false);
        }

        // Only trigger the visual reset locally if we have authority, or manage visuals locally per player
        if (onResetTracking != null)
        {
            onResetTracking.Invoke();
        }
    }

    IEnumerator UpdateCoordinatesLoop()
    {
        while (true)
        {
            if (_isSnappedAndLocked) yield break;

            // CRITICAL MULTIPLAYER CHECK: Only the player with State Authority tracks the headset!
            if (Object != null && Object.HasStateAuthority)
            {
                UpdateCoordinates();
            }
            else
            {
                // If we are another player, we don't run headset tracking math.
                // We just let the NetworkTransform component automatically slide the table across our screen.
                if (debugText != null) debugText.text = "Viewing networked table movement...";
            }

            yield return new WaitForSeconds(0.33f);
        }
    }

    void UpdateCoordinates()
    {
        // [The rest of your tracking logic stays exactly the same as before]
        OVRCameraRig rig = FindFirstObjectByType<OVRCameraRig>();
        if (rig == null) return; 

        Transform headset = rig.centerEyeAnchor;
        Vector3 targetPos = headset.position + headset.rotation * tableOffset;
        Vector3 finalTablePos = new Vector3(targetPos.x, table.position.y, targetPos.z);

        if (!_hasStartPosition)
        {
            _startPosition = finalTablePos;
            table.position = _startPosition;
            
            Vector3 lookDirection = Vector3.ProjectOnPlane(headset.forward, Vector3.up).normalized;
            Vector3 xSpawnPosition = _startPosition + (lookDirection * targetDistance);

            if (targetDestination != null)
            {
                targetDestination.position = new Vector3(xSpawnPosition.x, targetDestination.position.y, xSpawnPosition.z);
                targetDestination.rotation = Quaternion.Euler(0, headset.rotation.eulerAngles.y, 0); 
                targetDestination.gameObject.SetActive(true); 
            }

            _hasStartPosition = true;
        }

        if (targetDestination != null && !_hasTriggeredReveal)
        {
            Vector3 targetNoY = new Vector3(targetDestination.position.x, 0, targetDestination.position.z);
            Vector3 tableNoY = new Vector3(finalTablePos.x, 0, finalTablePos.z);
            
            float distanceToTarget = Vector3.Distance(tableNoY, targetNoY);

            if (debugText != null)
            {
                debugText.text = $"\n Table position: {table.position}\n Distance to X: {distanceToTarget:F2}m";
            }

            if (distanceToTarget <= snapDistance)
            {
                table.position = new Vector3(targetDestination.position.x, table.position.y, targetDestination.position.z);
                table.rotation = Quaternion.Euler(0, targetDestination.rotation.eulerAngles.y, 0);
                
                _isSnappedAndLocked = true;
                TriggerReveal();
                
                if (debugText != null) debugText.text = "Table arrived at X! Room revealed & Locked.";
                return; 
            }
        }

        table.position = finalTablePos;
        Vector3 euler = headset.rotation.eulerAngles;
        table.rotation = Quaternion.Euler(0, euler.y, 0);
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