using UnityEngine;

public class defaultSV : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MQTTProcessor.Instance.PublishMessage("MMYC/language", "1");
    }
}
