using System;
using UnityEngine;

namespace Supercent.Util
{
    public static class MathUtil
    {
        public static float DivRem(float a, float b, out float result)
        {
            var div = (float)Math.Floor(a / b);
            result = a - (div * b);
            return div;
        }


        public static float Calc_Percent_Between_A_and_B(float a, float b, float factor)
        {
            if (Mathf.Approximately(a, b))
            {
                if (factor < a) return 0.0f;
                else            return 1.0f;
            }

            var deltaBA         = b - a;
            var deltaFactorA    = factor - a;

            return Mathf.Clamp01(deltaFactorA / deltaBA);
        }

        public static float Lerp_Percent_Between_A_and_B(float a, float b, float factor, float aValue, float bValue)
        {
            var t = Calc_Percent_Between_A_and_B(a, b, factor);
            return Mathf.Lerp(aValue, bValue, t);
        }
    }
}