using System.Collections.Generic;
using Unity.Mathematics;

public static class ScaleDef
{
    public enum WeightType
    {
        None,
        Skull,
        Cat,
        Pyramid,
        Eye
    }

    public static Dictionary<WeightType, float> Weight = new()
    {
        { WeightType.None, 0f },
        { WeightType.Skull, 3.5f },
        { WeightType.Cat, 2.5f },
        { WeightType.Pyramid, 4.5f },
        { WeightType.Eye, 1.5f }
    };

    public static float LeftDefaultWeight = 1f;
    public static float RightDefaultWeight = 5f;

    public static WeightType GetWeightTypeBySerial(int num)
    {
        throw new System.NotImplementedException();
        // TODO
    }

    public static float GetWeightBySerial(int num)
    {
        throw new System.NotImplementedException();
        // TODO
    }
}