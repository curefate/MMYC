using System.Collections;
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

    private Coroutine calibrationCoroutine;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            calibrationCoroutine = StartCoroutine(Routine_Calibration());
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (MQTTProcessor.Instance.CheatCode == 9)
        {
            if (calibrationCoroutine != null)
            {
                StopCoroutine(calibrationCoroutine);
            }
            calibrationCoroutine = StartCoroutine(Routine_Calibration());
        }

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
        if (MQTTProcessor.Instance.CheatCode == 5)
        {
            diff = 0;
        }
        if (diff == 0 && currentWeightDifference != 0)
        {
            RPC_OnEqual();
        }
        currentWeightDifference = diff;

        Angle = Mathf.Clamp(currentWeightDifference * anglePerWeight, -MaxAngle, MaxAngle);
        Debug.Log($"Left: {weight_l}, Right: {weight_r}, Angle: {Angle}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnEqual()
    {
        OnEqual.Invoke();
    }

    private IEnumerator Routine_Calibration()
    {
        yield return new WaitForSeconds(3f);
        var hall4 = MQTTProcessor.Instance.Hall_4;
        var baseLine = (MQTTProcessor.Instance.Hall_0 + MQTTProcessor.Instance.Hall_1 + MQTTProcessor.Instance.Hall_2 + MQTTProcessor.Instance.Hall_3 + hall4) / (4f + (hall4 == 0 ? 0f : 1f));
        MQTTProcessor.Instance.PublishMessage("MMYC/hall_base", Mathf.RoundToInt(baseLine).ToString());
        yield return null;
    }
}
