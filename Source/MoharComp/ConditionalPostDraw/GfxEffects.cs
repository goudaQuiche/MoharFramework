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
        static float RawRythm(this Thing thing)
        {
            return Time.realtimeSinceStartup + 397f * (thing.thingIDNumber % 571) * 40;
        }
        public static float VanillaPulse(this Thing thing)
        {
            float num = thing.VanillaRythm();
            float num2 = ((float)Math.Sin((double)num) + 1f) * 0.5f;
            num2 = 0.3f + num2 * 0.7f;
            return num2;
        }

        // mask = 1 opacity ; mask 360 rotation
        public static float LoopFactorOne(this Thing thing, float mask = 1, float speed = 1, bool debug = false)
        {
            float num = thing.VanillaRythm() * speed;
            float num2 = ((float)Math.Tan((double)num) + 1f) * 0.5f;
            //num2 = (0.3f + num2 * 0.7f)%1;
            Tools.Warn("loop factor one" + num2 + "; mask: " + mask + "; masked: " + num % mask, debug);
            num2 = num2 % mask;
            return num2;
        }


        // mask = 1 opacity ; mask 360 rotation
        public static float LinearLoop(this Thing thing, float mask = 1, float speed = 1, bool debug = false)
        {
            float num = thing.VanillaRythm();
            //float num2 = num % granularity;

            float result = (num * speed) % mask;
            Tools.Warn(
                "AnotherLoop "+
                "rythm:" + num +
                "; mask: " + mask +
                "; speed: " + speed +
                "; result: " + result,
                debug
            );
            
            return result;
        }

        public static float MirrorLoop(this Thing thing, float mask = 1, float speed = 1, bool debug = false)
        {
            float num = thing.VanillaRythm();
            //float num2 = num % granularity;

            float result = (num * speed) % mask;
            if (result > mask / 2)
                result = mask - result;
            Tools.Warn(
                "AnotherLoop " +
                "rythm:" + num +
                "; mask: " + mask +
                "; speed: " + speed +
                "; result: " + result,
                debug
            );

            return result;
        }

        // 0..360;0...
        public static float Loop360(this Thing thing, float speed = 1, bool debug = false)
        {
            return thing.LinearLoop(360, 30f * speed, debug);
        }

        public static float Mirror1(this Thing thing, float speed = 1, bool debug = false)
        {
            return thing.MirrorLoop(1, .35f * speed, debug);
        }
    }
}
