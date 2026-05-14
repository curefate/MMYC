using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class BeamRotator : NetworkBehaviour
{
    public int Speed = 10;
    public float MaxAngle = 25f;
    public float MaxWeightDifference = 7f;
    public UnityEvent OnEqual;

    [Networked]
    public float Angle { get; private set; }

    private float visualAngle;
    private float anglePerWeight => MaxAngle / MaxWeightDifference;
    private float currentWeightDifference;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        UpdateAngle();
    }

    public override void Render()
    {
        visualAngle = Mathf.Lerp(visualAngle, Angle, Speed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, visualAngle);
    }

    private void UpdateAngle()
    {
        if (!Object.HasStateAuthority)
            return;

        float l0 = ScaleDef.GetWeightBySerial(MQTTProcessor.Instance.Hall_0);
        float l1 = ScaleDef.GetWeightBySerial(MQTTProcessor.Instance.Hall_1);
        float r0 = ScaleDef.GetWeightBySerial(MQTTProcessor.Instance.Hall_2);
        float r1 = ScaleDef.GetWeightBySerial(MQTTProcessor.Instance.Hall_3);
        float weight_l = l0 + l1 + ScaleDef.LeftDefaultWeight;
        float weight_r = r0 + r1 + ScaleDef.RightDefaultWeight;

        float diff = weight_l - weight_r;
        if (diff == 0 && currentWeightDifference != 0)
        {
            OnEqual.Invoke();
        }
        currentWeightDifference = diff;

        Angle = Mathf.Clamp(currentWeightDifference * anglePerWeight, -MaxAngle, MaxAngle);
        Debug.Log($"Left: {weight_l}, Right: {weight_r}, Angle: {Angle}");
    }
}
