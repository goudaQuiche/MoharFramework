//using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
//using System.Collections.Generic;

using UnityEngine;


using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

using RimWorld;


namespace MoharBlood
{
    public class HarmonyPatch_Verse_Graphic
    {
        private static readonly string drawPatchName = nameof(Verse_Graphic_HarmonyPatch.Verse_Graphic_Print_Prefix);

        private static readonly Type patchType = typeof(Verse_Graphic_HarmonyPatch);
        private static readonly Type patchUtilsType = typeof(Filth_AddSources_Utils);
        private static readonly Type patchHarmonyUtilsType = typeof(Harmony_Utils);

        // RimWorld HealthCardUtility DrawHediffRow
        public static bool Try_GraphicPrint_Patch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(Verse.Graphic), "Print");

                HarmonyMethod Prefix = new HarmonyMethod(patchType, drawPatchName);
                HarmonyMethod Postfix = null;
                myPatch.Patch(Method, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + drawPatchName + " failed  - " + e);
                return false;
            }
            return true;
        }

        public static class Verse_Graphic_HarmonyPatch
        {
            public static bool Verse_Graphic_Print_Prefix(SectionLayer layer, Thing thing, Graphic __instance)
            {
                //Log.Warning("AddSources_Postfix");
                if (!(thing is ColorableFilth cf))
                    return true;

                //Log.Warning("Found Print");
                //+ "\n" + $"{loc} {rot} {thing} {extraRotation}");
                //Log.Warning($"Graphic_Print offsetDrawPos:{cf.offsetDrawPos} randomDrawSize:{cf.randomDrawSize} randomRotation:{cf.randomRotation}");

                //Printer_Plane.PrintPlane(layer, cf.offsetDrawPos, cf.randomDrawSize, __instance.MatAt(thing.Rotation, thing), cf.randomRotation);
                MyPrint(__instance, layer, thing);
                return false;
            }

            public static void MyPrint(Graphic graphic, SectionLayer layer, Thing thing)
            {
                const float PositionVariance = 0.45f;
                const float SizeFactorMin = 0.8f;
                const float SizeFactorMax = 1.2f;

                Vector3 a = thing.TrueCenter();
                Rand.PushState();
                Rand.Seed = thing.Position.GetHashCode();
                int num = (thing as Filth)?.thickness ?? 3;
                for (int i = 0; i < num; i++)
                {
                    Material matSingle = graphic.MatSingle;
                    Vector3 center = a + new Vector3(Rand.Range(-PositionVariance, PositionVariance), 0f, Rand.Range(-PositionVariance, PositionVariance));
                    Vector2 size = new Vector2(Rand.Range(graphic.data.drawSize.x * SizeFactorMin, graphic.data.drawSize.x * SizeFactorMax), Rand.Range(graphic.data.drawSize.y * SizeFactorMin, graphic.data.drawSize.y * SizeFactorMax));
                    float rot = Rand.RangeInclusive(0, 360);
                    bool flipUv = Rand.Value < 0.5f;
                    Printer_Plane.PrintPlane(layer, center, size, matSingle, rot, flipUv);
                }
                Rand.PopState();
            }
        }
    }
}
