using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool BetweenValuesCheck(float minValue, float maxValue, float target)
    {
        return (target > minValue && target < maxValue);
    }
}

