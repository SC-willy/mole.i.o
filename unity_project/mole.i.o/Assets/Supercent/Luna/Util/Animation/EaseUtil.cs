using System;
using UnityEngine;

namespace Supercent.Util
{
    public enum EaseType
    {
        Linear = 0,

        SineIn,
        SineOut,

        QuadIn,
        QuadOut,

        CubicIn,
        CubicOut,

        QuartIn,
        QuartOut,

        QuintIn,
        QuintOut,

        ExpoIn,
        ExpoOut,

        CircIn,
        CircOut,

        BackIn,
        BackOut,

        ElasticIn,
        ElasticOut,

        BounceIn,
        BounceOut,
    }

    public static class EaseUtil
    {
        const float PI_HALF = Mathf.PI / 2f;
        const float TAU = Mathf.PI * 2f;

        public static float Interpolate(EaseType type, float t) => Functions[(int)type](t);
        public static float Interpolate(EaseType typeA, EaseType typeB, float t)
        {
            return t < 0.5f
                 ? Functions[(int)typeA](t * 2f) * 0.5f
                 : Functions[(int)typeB]((t * 2f) - 1f) * 0.5f + 0.5f;
        }
        public static float Interpolate(EaseType typeA, EaseType typeB, float t, float p)
        {
            if (t < p)
                return Functions[(int)typeA](t / p) * p;

            var t2 = 1f - p;
            return Functions[(int)typeB]((t - p) / t2) * t2 + p;
        }
        public static Func<float, float>[] Functions = new Func<float, float>[]
        {
            Linear,

            SineIn,
            SineOut,

            QuadIn,
            QuadOut,

            CubicIn,
            CubicOut,

            QuartIn,
            QuartOut,

            QuintIn,
            QuintOut,

            ExpoIn,
            ExpoOut,

            CircIn,
            CircOut,

            BackIn,
            BackOut,

            ElasticIn,
            ElasticOut,

            BounceIn,
            BounceOut,
        };


        public static float Linear(float t)                 => t;


        public static float SineIn(float t)                 => (float)(1 - Math.Cos(t * PI_HALF));
        public static float SineOut(float t)                => (float)Math.Sin(t * PI_HALF);


        public static float QuadIn(float t)                 => t * t;
        public static float QuadOut(float t)                => -(t * (t - 2));


        public static float CubicIn(float t)                => (float)Math.Pow(t, 3);
        public static float CubicOut(float t)               => ((t -= 1) * t * t) + 1;


        public static float QuartIn(float t)                => (float)Math.Pow(t, 4);
        public static float QuartOut(float t)               => -(((t -= 1) * (float)Math.Pow(t, 3)) - 1);


        public static float QuintIn(float t)                => (float)Math.Pow(t, 5);
        public static float QuintOut(float t)               => ((t -= 1) * (float)Math.Pow(t, 4)) + 1;


        public static float ExpoIn(float t)                 => (float)Math.Pow(2, 10 * (t - 1));
        public static float ExpoOut(float t)                => (float)-Math.Pow(2, -10 * t) + 1;


        public static float CircIn(float t)                 => (float)-(Math.Sqrt(1 - (t * t)) - 1);
        public static float CircOut(float t)                => (float)Math.Sqrt(1 - ((t -= 1) * t));


        public static float BackIn(float t)                 => t * t * ((2.70158f * t) - 1.70158f);
        public static float BackOut(float t)                => ((t -= 1) * t * ((2.70158f * t) + 1.70158f)) + 1;


        public static float ElasticIn(float t)
        {
            float p = 0.3f;
            float s = p * 0.25f;
            return (float)-(Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t - s) * TAU / p));
        }
        public static float ElasticOut(float t)
        {
            float p = 0.3f;
            float s = p * 0.25f;
            return (float)((Math.Pow(2, -10 * t) * Math.Sin((t - s) * TAU / p)) + 1);
        }


        public static float BounceIn(float t)               => 1 - BounceOut(1 - t);
        public static float BounceOut(float t)
        {
            if (t < 0.363636f)      return 7.5625f * t * t;
            else if (t < 0.727273f) return (7.5625f * (t -= 0.545455f) * t) + 0.75f;
            else if (t < 0.909091f) return (7.5625f * (t -= 0.818182f) * t) + 0.9375f;
            else                    return (7.5625f * (t -= 0.954545f) * t) + 0.984375f;
        }
    }
}