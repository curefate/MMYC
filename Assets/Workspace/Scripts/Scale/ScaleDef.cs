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
        if (num > 2400 && num <= 2600)
            return WeightType.Eye;
        else if (num > 3300 && num <= 3600)
            return WeightType.Cat;
        else if (num > 4900 && num <= 5200)
            return WeightType.None;
        else if (num > 6300 && num <= 6600)
            return WeightType.Skull;
        else if (num > 7800 && num <= 8000)
            return WeightType.Pyramid;
        else
            return WeightType.None;
    }

    public static float GetWeightBySerial(int num)
    {
        return WeightDict[GetWeightTypeBySerial(num)];
    }
}