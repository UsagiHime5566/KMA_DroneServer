using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternFunc
{
    public static float CutFloat(float src){
        return Mathf.Round(src * 10f) / 10f;
    }

    public static float CalculateAngleDifferenceZ(float azimuthAngle)
    {
        // 以Z轴正向为基准
        float referenceAngle = 0f;
        // 计算差值（以180为周期）
        float angleDifference = (azimuthAngle - referenceAngle + 180f) % 360f - 180f;

        return angleDifference;
    }
}
