using System;
using UnityEngine;
using Verse;

namespace ConPoDra
{
    public static class GfxEffects
    {
        static float VanillaRythm(this Thing thing)
        {
            return (Time.realtimeSinceStartup + 397f * (thing.thingIDNumber % 571)) * 4f;
        }
        public static float VanillaPulse(this Thing thing)
        {
            float num = thing.VanillaRythm();
            float num2 = ((float)Math.Sin((double)num) + 1f) * 0.5f;
            num2 = 0.3f + num2 * 0.7f;
            return num2;
        }

    }
}
