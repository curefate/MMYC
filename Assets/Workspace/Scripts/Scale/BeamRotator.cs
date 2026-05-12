using Fusion;
using UnityEngine;

public class BeamRotator : NetworkBehaviour
{
    public int Speed = 10;
    public float MaxAngle = 25f;
    public float MaxWeightDifference = 7f;
    [Networked]
    public float Angle { get; private set; }

    private float anglePerWeight => MaxAngle / MaxWeightDifference;

    public override void FixedUpdateNetwork()
    {
        float targetAngle = Mathf.Lerp(transform.localEulerAngles.z, Angle, Speed * Runner.DeltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, targetAngle);

        if (!Object.HasStateAuthority)
            return;

        UpdateAngle();
    }

    private void UpdateAngle()
    {
        if (!Object.HasStateAuthority)
            return;
    }
}
