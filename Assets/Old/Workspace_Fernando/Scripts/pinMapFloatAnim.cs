using Fusion;
using UnityEngine;

public class pinMapFloatAnim : MonoBehaviour
{
    public float amplitude = 0.005f; // smaller movement
    public float speed = 2f;

    void Update()
    {
        float yOffset = (Mathf.Sin(Time.time * speed) + 1f) / 2f * amplitude;
        transform.position = transform.parent.transform.position + new Vector3(0, yOffset, 0);
        transform.Rotate(Vector3.forward, speed * Time.deltaTime * 50f);
    }
}