using UnityEngine;
using UnityEngine.Events;

public class TableDistanceTrigger : MonoBehaviour
{
    [Header("Distance Settings")]
    [Tooltip("Distance threshold in meters (0.3 = 30cm)")]
    [SerializeField] private float triggerDistance = 0.3f;
    
    [Header("Events to Fire")]
    public UnityEvent onDistanceReached;
    public UnityEvent onResetTracking;

    private Vector3 _startPosition;
    private bool _hasStartPosition = false;
    private bool _hasTriggered = false;

    void Start()
    {
        ResetTrigger();
    }

    public void ResetTrigger()
    {
        _hasTriggered = false;
        _hasStartPosition = false;
        
        if (onResetTracking != null)
        {
            onResetTracking.Invoke();
        }
    }

    void Update()
    {
        if (_hasTriggered) return;

        // Wait until the table is initialized by your main script
        if (!_hasStartPosition && transform.position != Vector3.zero)
        {
            _startPosition = new Vector3(transform.position.x, 0, transform.position.z);
            _hasStartPosition = true;
            return;
        }

        if (_hasStartPosition)
        {
            Vector3 currentPosNoY = new Vector3(transform.position.x, 0, transform.position.z);
            float distanceMoved = Vector3.Distance(_startPosition, currentPosNoY);

            if (distanceMoved >= triggerDistance)
            {
                _hasTriggered = true;
                if (onDistanceReached != null)
                {
                    onDistanceReached.Invoke();
                }
                Debug.Log("Table moved 30cm! Elements Revealed.");
            }
        }
    }
}