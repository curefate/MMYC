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
        { WeightType.Skull, 3.6f },
        { WeightType.Cat, 2.6f },
        { WeightType.Pyramid, 4.5f },
        { WeightType.Eye, 1.5f }
    };

    public static Dictionary<WeightType, int> WeightSerialCentralDict = new()
    {
        { WeightType.Skull, 1200 },
        { WeightType.Cat, -1300 },
        { WeightType.Pyramid, 2500 },
        { WeightType.Eye, -2150 },
        { WeightType.None, 0 }
    };

    public static int SerialRangeExpand = 550;
    public static float LeftDefaultWeight = 1f;
    public static float RightDefaultWeight = 5f;

    public static WeightType GetWeightTypeBySerial(int num)
    {
        var baseLine = MQTTProcessor.Instance.Hall_base;
        foreach (var kvp in WeightSerialCentralDict)
        {
            if (math.abs(num - baseLine - kvp.Value) <= SerialRangeExpand)
            {
                return kvp.Key;
            }
        }
        return WeightType.None;
    }

    public static float GetWeightBySerial(int num)
    {
        return WeightDict[GetWeightTypeBySerial(num)];
    }
}