using UnityEngine;
using Fusion;
using TMPro;

public class NetworkVariableTest : NetworkBehaviour
{
    [Networked]
    public int testValue { get; set; }

    public TMP_Text debugText;

    public void AddValue()
    {
        testValue++;
    }

    public override void Render()
    {
        if (debugText != null)
        {
            //debugText.text = "Value: " + testValue;
        }
    }
}