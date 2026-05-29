using UnityEngine;

public class ForceRotation : MonoBehaviour
{
    public Quaternion targetRotation;
    public float speed;

    void Update()
    {
        if (speed > 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}
