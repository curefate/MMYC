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

    public static Dictionary<WeightType, float> WeightDict = new()
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
        if (num <= 3000)
            return WeightType.Eye;
        else if (num > 3000 && num <= 4800)
            return WeightType.Cat;
        else if (num > 4800 && num <= 5800)
            return WeightType.None;
        else if (num > 5800 && num <= 7500)
            return WeightType.Skull;
        else if (num >= 7500)
            return WeightType.Pyramid;
        else
            return WeightType.None;
    }

    public static float GetWeightBySerial(int num)
    {
        return WeightDict[GetWeightTypeBySerial(num)];
    }
}