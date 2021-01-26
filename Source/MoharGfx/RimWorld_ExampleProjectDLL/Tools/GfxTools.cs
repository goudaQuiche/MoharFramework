using Verse;
using UnityEngine;
using System;

namespace MoharGfx
{
    public static class GfxTools
    {
        static float VanillaRythm(this Thing thing)
        {
            return Time.realtimeSinceStartup + 397f * (thing.thingIDNumber % 571);
        }

        public static float VanillaPulse(this Thing thing, float speed, float range)
        {
            float num = thing.VanillaRythm() * speed;
            float num2 = ((float)Math.Sin(num) + 1f) * 0.5f;
            num2 = num2 * range;
            return num2;
        }
    }
}
